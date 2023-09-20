﻿using Entities.DataModels;
using Entities.DtoModels.QueryModels;
using Repositories.Utilies;

namespace Repositories.Contracts
{
	public interface ICompanyRepository : IRepositoryBase<Company>
	{
		Task<Company?> GetCompanyByIdAsync(int id);
		Task<Company?> GetCompanyByNameAsync(string name);
		Task<PagingList<Company>> GetAllCompaniesAsync(
            PaginationQueryDto paginationQueryDto);
    }
}
