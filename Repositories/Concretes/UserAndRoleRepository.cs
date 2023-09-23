﻿using Entities.DataModels.RelationModels;
using Entities.ViewModels;
using Repositories.Contracts;

namespace Repositories.Concretes
{
    public class UserAndRoleRepository 
        : RepositoryBase<UserAndRole>, IUserAndRoleRepository
    {
        public UserAndRoleRepository(RepositoryContext context)
            : base(context)
        { }

        //public void CreateUserAndRole(UserAndRole userAndRole) =>
        //    base.Create(userAndRole);

        //public async Task<List<UserAndRole>> GetUserAndRolesByUserIdAsync(Guid? id) =>
        //    await base
        //        .DisplayByCondition<UserAndRole>(ur => ur.UserId.Equals(id))
        //        .ToListAsync();

        //public async Task<List<UserAndRole>> GetUserAndRolesByRoleIdAsync(int id) =>
        //        await base
        //            .DisplayByCondition<UserAndRole>(ur => ur.RoleId == id)
        //            .ToListAsync();

        //public async Task<List<string>> GetRoleNamesOfUserByUserIdAsync(Guid id) =>
        //    await base.ExecProcedureAsync<string>($@"
        //        EXEC User_GetRoleNamesOfUser
        //        @UserId = {id}");
    }
}
