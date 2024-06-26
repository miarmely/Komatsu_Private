﻿using Dapper;
using Entities.ConfigModels.Contracts;
using Entities.DtoModels;
using Entities.ViewModels;
using Repositories.Contracts;


namespace Repositories.Concretes
{
	public class UserRepository : RepositoryBase, IUserRepository
	{
		public UserRepository(RepositoryContext context, IConfigManager configs)
			: base(context, configs)
		{ }

		public async Task<UserView?> LoginAsync(
			DynamicParameters parameters)
		{
			#region login and get userInfos if login successfull
			UserView userView = null;

			try
			{
				await base.QueryAsync<UserView, RolePartForUserView, UserView>(
					base.Configs.DbSettings.ProcedureNames.U_Login,
					parameters,
					(userViewPart, RolePart) =>
					{
						#region populate roleNames of User 
						// when userView not initialized
						if (userView == null)
							userView = userViewPart;

						userView.RoleNames.Add(RolePart.RoleName);
						#endregion

						return userViewPart;
					},
					"RoleId");
			}

			catch (Exception ex)
			{
				// if expected error occured in database, multi-mapping method not mapping the error details from db and throw different error. So i put try-catch block and i passed this block
			}
			#endregion

			return userView;
		}

		public async Task<ErrorDtoWithMessage?> CreateUserAsync(DynamicParameters parameters) =>
			await base.QuerySingleOrDefaultAsync<ErrorDtoWithMessage>(
				base.Configs.DbSettings.ProcedureNames.U_Create,
				parameters);

		public async Task<IEnumerable<UserView>?> GetAllUsersWithPagingAsync(
			DynamicParameters parameters)
		{
			#region get userViews
			var userViewDict = new Dictionary<Guid, UserView>();

			var userViews = await base.QueryAsync
				<UserView, RolePartForUserView, UserView>(
					base.Configs.DbSettings.ProcedureNames.U_DisplayAll,
					parameters,
					(userViewPart, rolePart) =>
					{
						#region add userView to dict if not exists
						if (!userViewDict.TryGetValue(
							key: userViewPart.UserId,
							value: out var currentUserView))
						{
							currentUserView = userViewPart;
							userViewDict.Add(userViewPart.UserId, currentUserView);
						}
						#endregion

						#region add roleName to userView
						currentUserView.RoleNames.Add(rolePart.RoleName);
						#endregion

						return currentUserView;
					},
					"RoleId");
			#endregion

			return userViewDict.Values;
		}

		public async Task<IEnumerable<string>> GetAllRolesByLanguageAsync(
			DynamicParameters parameters)
			=> await base.QueryAsync<string>(
					base.Configs.DbSettings.ProcedureNames.U_DisplayAllRoles,
					parameters);

		public async Task<UserView?> GetUserByTelNoAsync(DynamicParameters parameters)
		{
			#region get userView
			UserView? userView = null;

			await base.QueryAsync<UserView, RolePartForUserView, UserView>(
				base.Configs.DbSettings.ProcedureNames.U_DisplayByTelNo,
				parameters,
				(userViewPart, rolePart) =>
				{
					#region populate userView
					userView = userViewPart;

					// add roleNames
					userView.RoleNames.Add(rolePart.RoleName);
					#endregion

					return userView;
				},
				"RoleId");
			#endregion

			return userView;
		}

		public async Task<ErrorDtoWithMessage?> UpdateUserByTelNoAsync(
			DynamicParameters parameters) =>
				await base.QuerySingleOrDefaultAsync<ErrorDtoWithMessage>(
					base.Configs.DbSettings.ProcedureNames.U_Update,
					parameters);

		public async Task<ErrorDtoWithMessage?> DeleteUsersByTelNoListAsync(
			DynamicParameters parameters) =>
				await base.QuerySingleOrDefaultAsync<ErrorDtoWithMessage>(
					base.Configs.DbSettings.ProcedureNames.U_MultipleDeleteByTelNo,
					parameters);

		public async Task<ErrorDtoWithMessage> CloseAccountAsync(
			DynamicParameters parameters) =>
				await ExecuteAsync(
					Configs.DbSettings.ProcedureNames.U_CloseAccount,
					parameters);
	}
}