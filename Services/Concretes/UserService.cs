﻿using AutoMapper;
using Entities.ConfigModels.Contracts;
using Entities.DataModels.RelationModels;
using Entities.DtoModels;
using Entities.DtoModels.BodyModels;
using Entities.DtoModels.QueryModels;
using Entities.Exceptions;
using Entities.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Repositories.Contracts;
using Services.Contracts;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Services.Concretes
{
    public class UserService : IUserService
	{
		private readonly IRepositoryManager _manager;
		private readonly IConfigManager _config;
		private readonly IMapper _mapper;
		private readonly IDtoConverterService _dtoConverterService;

        public UserService(IRepositoryManager manager,
			IConfigManager config,
			IMapper mapper,
			IDtoConverterService dtoConverterService)
		{
			_manager = manager;
			_config = config;
			_mapper = mapper;
			_dtoConverterService = dtoConverterService;
		}

		public async Task<string> LoginAsync(UserBodyDtoForLogin UserDtoL)
		{
			#region get userView by telNo
			var userView = await _manager.UserRepository
				.GetUserByTelNoAsync(UserDtoL.TelNo);
			#endregion

			#region when telNo not found (throw)
			_ = userView ?? throw new ErrorWithCodeException(404,
				"VE-U-T",
				"Verification Error - Telephone");
			#endregion

			#region when password is wrong (throw)
			var hashedPassword = await ComputeMd5Async(UserDtoL.Password);

			if (!userView.Password.Equals(hashedPassword))
				throw new ErrorWithCodeException(404,
					"VE-U-P",
					"Verification Error - User - Password");
			#endregion

			return await GenerateTokenForUserAsync(userView);
		}

		public async Task CreateUserAsync(UserBodyDtoForCreate userDtoC, string roleName)
		{
            #region set variables
            var hashedPassword = await ComputeMd5Async(userDtoC.Password);
			var errorCode = "";
			#endregion

			#region create user
			await _manager.UserRepository
				.ExecProcedureAsync<string>($@"EXEC CreateUser 
					@FirstName = ${userDtoC.FirstName},
					@LastName = ${userDtoC.LastName},
					@CompanyName = ${userDtoC.CompanyName},
					@TelNo = ${userDtoC.TelNo},
					@Email = ${userDtoC.Email},
					@Password = ${hashedPassword},
					@RoleName = ${roleName}
					@ErrorCode = ${errorCode} out");
			#endregion
		}

        public async Task<ICollection<UserView>> GetAllUsersAsync(
			PaginationQueryDto pagingParameters, 
			HttpResponse response)
		{
			#region when user not found (throw)
			var userViewList = await _manager.UserRepository
				.GetAllUsersAsync(pagingParameters);

			if (userViewList.Count == 0)
				throw new ErrorWithCodeException(404, 
					"NF-U", 
					"Not Found - User");
			#endregion

			#region add pagination details to headers
			response.Headers.Add(
				"User-Pagination", 
				userViewList.GetMetaDataForHeaders());
			#endregion

			return userViewList;
		}

		public async Task UpdateUserAsync(string email, UserBodyDtoForUpdate userDtoU)
		{
			#region control conflict error
			var userDtoC = _mapper.Map<UserDtoForConflictControl>(userDtoU);

			// if values null, then they didn't change.
			await ConflictControlAsync(u =>
				u.TelNo == null ? true : u.TelNo == userDtoU.TelNo
				|| u.Email == null ? true : u.Email == userDtoU.Email,
				userDtoC);
			#endregion

			#region when user not found (throw)
			var user = await _manager.UserRepository
				.GetUserByEmailAsync(email);

			if (user == null)
				throw new ErrorWithCodeException(404,
					"VE-U-E",
					"Verification Error - Email");
			#endregion

			#region get company
			var company = userDtoU.CompanyName == null ?
				null  // if company name not updated
				: await GetCompanyAndCreateIfNotExistsAsync(userDtoU.CompanyName);
			#endregion

			#region update user
			user.FirstName = userDtoU.FirstName ?? user.FirstName;
			user.LastName = userDtoU.LastName ?? user.LastName;
			user.TelNo = userDtoU.TelNo ?? user.TelNo;
			user.CompanyId = company == null ? user.CompanyId : company.Id;
			user.Email = userDtoU.Email ?? user.Email;

			_manager.UserRepository.Update(user);
			#endregion

			#region create&delete userAndRole
			if (userDtoU.RoleNames != null)
			{
				#region delete userAndRole that not exists userDto
				var userAndRoles = await _manager.UserAndRoleRepository
					.GetUserAndRolesByUserIdAsync(user.Id);

				#region delete UserAndRole 
				var roleNamesInDatabase = new Collection<string>();

				foreach (var userAndRole in userAndRoles)
				{
					#region get role
					var role = await _manager.RoleRepository
						.GetRoleByIdAsync(userAndRole.RoleId);

					roleNamesInDatabase.Add(role.Name);
					#endregion

					#region when exists database but not exists current data
					if (!userDtoU.RoleNames.Contains(role.Name))
						_manager.UserAndRoleRepository
							.Delete(userAndRole);
					#endregion
				}
				#endregion

				#endregion

				#region create userAndRole that not exists database
				foreach (var roleNameInDto in userDtoU.RoleNames)
				{
					// when not exists database but exists userDto
					if (!roleNamesInDatabase.Contains(roleNameInDto))
					{
						#region get role
						var role = await _manager.RoleRepository
							.GetRoleByNameAsync(roleNameInDto);
						#endregion

						#region create userAndRole
						_manager.UserAndRoleRepository
							.Create(new UserAndRole
							{
								UserId = user.Id,
								RoleId = role.Id
							});
						#endregion
					}
				}
				#endregion
			}

			await _manager.SaveAsync();
			#endregion
		}

		public async Task DeleteUsersAsync(UserBodyDtoForDelete userDtoD)
		{
			#region delete users
			var IsFalseTelNoExists = false;

			foreach (var telNo in userDtoD.TelNos)
			{
				#region get user by telNo
				var user = await _manager.UserRepository
					.GetUserByTelNoAsync(telNo);
				#endregion

				#region when telNo not found
				if (user == null)
				{
					IsFalseTelNoExists = true;
					break;
				}
				#endregion

				#region delete user
				_manager.UserRepository
					.Delete(user);
				#endregion

				#region delete userAndRoles of user
				var userAndRoles = await _manager.UserAndRoleRepository
					.GetUserAndRolesByUserIdAsync(user.Id);

				userAndRoles.ForEach(userAndRole =>
					_manager.UserAndRoleRepository
						.Delete(userAndRole)
				);
				#endregion
			}
			#endregion

			#region when any telNo not Found (throw)
			if (IsFalseTelNoExists)
				throw new ErrorWithCodeException(409,
					"VE-U-T",
					"Verification Error - Telephone");
			#endregion

			await _manager.SaveAsync();
		}


		#region private

		private async Task ConflictControlAsync(
			Expression<Func<UserView, bool>> condition,
			string telNo, 
			string email)
		{
			#region get users
			var userViewList = await _manager.UserRepository
				.GetUsersByConditionAsync(condition);
			#endregion

			#region control conflict error (throw)
			if (userViewList.Count != 0)
			{
				#region control telNo
				if (userViewList.Any(u => u.TelNo.Equals(telNo)))
				{
					_conflictErrorCode += "T";
					_conflictErrorDescription += "TelNo ";
				}
				#endregion

				#region control email
				if (userViewList.Any(u => u.Email.Equals(email)))
				{
                    _conflictErrorCode += "E";
                    _conflictErrorDescription += "Email ";
				}
                #endregion

                #region throw exception
                _conflictErrorDescription = _conflictErrorDescription.TrimEnd();

				throw new ErrorWithCodeException(409, 
					_conflictErrorCode, 
					_conflictErrorDescription);
				#endregion
			}
			#endregion
		}

		private async Task<string> ComputeMd5Async(string input) =>
			await Task.Run(() =>
			{
				using (var md5 = MD5.Create())
				{
					#region hash to input
					var hashInBytes = md5.ComputeHash(Encoding.UTF8
						.GetBytes(input));

					var hashAsString = Convert.ToBase64String(hashInBytes);
					#endregion

					return hashAsString;
				}
			});

		private async Task<string> GenerateTokenForUserAsync(UserView userView)
		{
			#region set claims
			var claims = new Collection<Claim>
			{
				new Claim("Id", userView.Id.ToString()),
				new Claim("FirstName", userView.FirstName),
				new Claim("LastName", userView.LastName)
			};

			#region add roles
			var roleNames = await _manager.UserAndRoleRepository
				.GetRoleNamesOfUserByUserIdAsync(userView.Id);

			foreach (var roleName in roleNames)
				claims.Add(
					new Claim("Role", roleName));
			#endregion

			#endregion

			#region set signingCredentials
			var secretKeyInBytes = Encoding.UTF8
					.GetBytes(_config.JwtSettings.SecretKey);

			var signingCredentials = new SigningCredentials(
				new SymmetricSecurityKey(secretKeyInBytes),
				SecurityAlgorithms.HmacSha256);
			#endregion

			#region set token
			var token = new JwtSecurityToken(
				issuer: _config.JwtSettings.ValidIssuer,
				audience: _config.JwtSettings.ValidAudience,
				claims: claims,
				expires: DateTime.Now.AddMinutes(_config.JwtSettings.Expires),
				signingCredentials: signingCredentials);
			#endregion

			return new JwtSecurityTokenHandler()
				.WriteToken(token);
		}
	}
}