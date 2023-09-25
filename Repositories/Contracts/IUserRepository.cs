﻿using Dapper;
using Entities.DtoModels;
using Entities.QueryModels;
using Entities.ViewModels;

namespace Repositories.Contracts
{
    public interface IUserRepository : IRepositoryBase<UserView>
    {
        Task<ErrorDto?> CreateUserAsync(DynamicParameters parameters);
        Task<ErrorDto?> UpdateUserByTelNoAsync(DynamicParameters parameters);
        Task DeleteUsersByTelNoListAsync(IEnumerable<string> telNoList);
        Task<IEnumerable<UserView>?> GetAllUsersAsync();
        Task<UserView?> GetUserByTelNoAsync(DynamicParameters parameters);

        Task<PagingList<UserView>?> GetAllUsersWithPagingAsync(
            PaginationParameters paginationQueryDto);


  //      Task<List<UserView>> GetUsersByConditionAsync(
  //          Expression<Func<UserView, bool>> condition);

  //      Task<List<UserView>> GetUsersByConditionAsync(
  //          PaginationQueryDto paginationQueryDto,
  //          Expression<Func<UserView, bool>> condition);

		//Task<List<UserView>> GetUsersByConditionAsync<T>(
  //          PaginationQueryDto paginationQueryDto,
  //          Expression<Func<UserView, bool>> condition, 
		//	Expression<Func<UserView, T>> orderBy, 
		//	bool asAscending = true);		
	}
}
