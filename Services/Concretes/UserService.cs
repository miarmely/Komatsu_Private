﻿using AutoMapper;
using Entities.ConfigModels.Contracts;
using Entities.DataModels;
using Entities.DtoModels.User;
using Entities.ErrorModels;
using Entities.Exceptions;
using Entities.RelationModels;
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
		private readonly IRepositoryManager _repository;
		private readonly IConfigManager _config;
		private readonly IMapper _mapper;
		private readonly IDataConverterService _dataConverterService;

		public UserService(IRepositoryManager repository,
			IConfigManager config,
			IMapper mapper,
			IDataConverterService dataConverterService)
		{
			_repository = repository;
			_config = config;
			_mapper = mapper;
			_dataConverterService = dataConverterService;
		}

		public async Task<string> LoginAsync(UserDtoForLogin UserDtoL)
		{
			#region get user by telNo
			var user = await _repository.UserRepository
				.GetUserByTelNoAsync(UserDtoL.TelNo, false);
			#endregion

			#region when telNo not found (throw)
			_ = user ?? throw new ErrorWithCodeException(404,
				"VE-T",
				"Verification Error - Telephone");
			#endregion

			#region when password is wrong (throw)
			var hashedPassword = await ComputeMd5Async(UserDtoL.Password);

			if (!user.Password.Equals(hashedPassword))
				throw new ErrorWithCodeException(404,
					"VE-P",
					"Verification Error - Password");
			#endregion

			return await GenerateTokenForUserAsync(user.Id);
		}

		public async Task RegisterAsync(UserDtoForRegisterWithoutRole userDtoR, 
			string roleName)
		{
			#region control conflict errors
			await ConflictControlAsync(u =>
				u.TelNo.Equals(userDtoR.TelNo)  // telNo
				|| u.Email.Equals(userDtoR.Email)  // email
				, userDtoR);
			#endregion

			#region get company
			var company = await _repository.CompanyRepository
				.GetCompanyByNameAsync(userDtoR.CompanyName, false);

			#region create company if not exists on database
			if (company == null)
			{
				company = new Company()
				{
					Name = userDtoR.CompanyName
				};

				_repository.CompanyRepository
					.Create(company);

				await _repository.SaveAsync();
			}
			#endregion

			#endregion

			#region convert userDtoR to user
			var user = _mapper.Map<User>(userDtoR);

			user.CompanyId = company.Id;
			user.Password = await ComputeMd5Async(user.Password);
			#endregion

			#region create user
			_repository.UserRepository
				.Create(user);

			await _repository.SaveAsync();
			#endregion

			#region create userAndRole
			var role = await _repository.RoleRepository
				.GetRoleByNameAsync(roleName, false);

			// create,
			var entity = new UserAndRole()
			{
				UserId = user.Id,
				RoleId = role.Id
			};

			_repository.UserAndRoleRepository
				.Create(entity);

			await _repository.SaveAsync();
			#endregion
		}

		private async Task ConflictControlAsync(
			Expression<Func<User, bool>> forWhichKeys,
			UserDtoForRegisterWithoutRole userDtoR)
		{
			#region get users
			var users = await _repository.UserRepository
				.GetUsersByConditionAsync(forWhichKeys);
			#endregion

			#region control conflict error
			if (users.Count != 0)
			{
				var errorModel = new ErrorDetails()
				{
					StatusCode = 409,
					ErrorCode = "CE-",
					ErrorDescription = "Conflict Error - ",
				};

				#region when telNo already exists
				if (users.Any(u => u.TelNo.Equals(userDtoR.TelNo)))
					UpdateErrorCode(ref errorModel, "T", "TelNo ");
				#endregion

				#region when email already exists
				if (users.Any(u => u.Email.Equals(userDtoR.Email)))
					UpdateErrorCode(ref errorModel, "E", "Email ");
				#endregion

				#region throw exception
				errorModel.ErrorDescription = errorModel.ErrorDescription.TrimEnd();
				throw new ErrorWithCodeException(errorModel);
				#endregion
			}
			#endregion
		}

		private void UpdateErrorCode(ref ErrorDetails errorModel
			, string newErrorCode
			, string newErrorDescription)
		{
			errorModel.ErrorCode += newErrorCode;
			errorModel.ErrorDescription += newErrorDescription;
		}

		private async Task<ICollection<string>> GetRoleNamesOfUserAsync(Guid userId)
		{
			#region get userAndRoles
			var userAndRoles = await _repository.UserAndRoleRepository
				.GetUserAndRolesByUserIdAsync(userId, false);
			#endregion

			#region add roleNames to collection
			var roleNames = new Collection<string>();
			Role role;

			foreach (var userAndRole in userAndRoles)
			{
				role = await _repository.RoleRepository
					.GetRoleByIdAsync(userAndRole.RoleId, false);

				roleNames.Add(role.Name);
			}
			#endregion

			return roleNames;
		}

		private async Task<string> ComputeMd5Async(string input) =>
			await Task.Run(() =>
			{
				using (var md5 = MD5.Create())
				{
					#region hash to input
					var hashAsByte = md5.ComputeHash(Encoding.UTF8
						.GetBytes(input));

					var hashAsString = Convert.ToBase64String(hashAsByte);
					#endregion

					return hashAsString;
				}
			});

		private async Task<string> GenerateTokenForUserAsync(Guid userId)
		{
			#region set claims
			var claims = new Collection<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userId.ToString())  // add userId
			};
			var roleNames = await GetRoleNamesOfUserAsync(userId);

			// add roles
			foreach (var roleName in roleNames)
				claims.Add(new Claim(ClaimTypes.Role, roleName));
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