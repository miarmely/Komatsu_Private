﻿using Entities.DataModels;
using Entities.DtoModels.QueryModels;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Repositories.EF;
using Repositories.Utilies;
using System.Linq.Expressions;

namespace Repositories.Concretes
{
    public class MachineRepository : RepositoryBase<Machine>, IMachineRepository
	{
		public MachineRepository(RepositoryContext context) : base(context)
		{ }

		public async Task<Machine?> GetMachineByMachineIdAsync(
			Guid machineId, 
			bool trackChanges) =>
				await base
					.FindWithCondition(m => m.Id.Equals(machineId), trackChanges)
					.SingleOrDefaultAsync();

		public async Task<Machine?> GetMachineByCategoryIdAndModelAsync(
			int categoryId,
			string model,
			bool trackChanges = false) =>
				await base
					.FindWithCondition(m => 
						m.CategoryId == categoryId
						&& m.Model.Equals(model), trackChanges)
					.SingleOrDefaultAsync();

		#region GetAllMachinesAsync
		public async Task<List<Machine>> GetAllMachinesAsync(
			bool trackChanges = false) =>
				await base
					.FindAll(trackChanges)
					.ToListAsync();
		/*
		 * with pagination
		 */
		public async Task<PagingList<Machine>> GetAllMachinesAsync(
			PaginationQueryDto pagingParameters,
			bool trackChanges = false) =>
				await PagingList<Machine>
					.ToPagingListAsync(
						base.FindAll(trackChanges),
						pagingParameters.PageNumber,
						pagingParameters.PageSize);
		/*
		 * with pagination + orderBy
		 */
		public async Task<PagingList<Machine>> GetAllMachinesAsync<TResult>(
			PaginationQueryDto pagingParameters,
			Expression<Func<Machine, TResult>> orderBy,
			bool asAscending = true,
			bool trackChanges = false) =>
				await PagingList<Machine>
					.ToPagingListAsync(
						asAscending ?
							base.FindAll(trackChanges).OrderBy(orderBy)
							: base.FindAll(trackChanges).OrderByDescending(orderBy),
						pagingParameters.PageNumber,
						pagingParameters.PageSize);
		#endregion

		#region GetMachinesByConditionAsync
		public async Task<List<Machine>> GetMachinesByConditionAsync(
			Expression<Func<Machine, bool>> condition,
			bool trackChanges = false) =>
				await base
				.FindWithCondition(condition, trackChanges)
				.ToListAsync();
		/*
		 * with pagination:
		 */
		public async Task<PagingList<Machine>> GetMachinesByConditionAsync(
			PaginationQueryDto paginationParameters,
			Expression<Func<Machine, bool>> condition,
			bool trackChanges = false) =>
				await PagingList<Machine>
					.ToPagingListAsync(
						base.FindWithCondition(condition, trackChanges),
						paginationParameters.PageNumber,
						paginationParameters.PageSize);
		/*
		 * with pagination + orderBy:
		 */
		public async Task<PagingList<Machine>> GetMachinesByConditionAsync<TResult>(
			PaginationQueryDto paginationParameters,
			Expression<Func<Machine, bool>> condition,
			Expression<Func<Machine, TResult>> orderBy,
			bool asAscending = true,
			bool trackChanges = false) =>
				asAscending ?
					await PagingList<Machine>
						.ToPagingListAsync(
							base.FindWithCondition(condition, trackChanges)
								.OrderBy(orderBy),
							paginationParameters.PageNumber,
							paginationParameters.PageSize)

					: await PagingList<Machine>
						.ToPagingListAsync(
							base.FindWithCondition(condition, trackChanges)
								.OrderByDescending(orderBy),
							paginationParameters.PageNumber,
							paginationParameters.PageSize);
		#endregion
	}
}
