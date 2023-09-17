﻿using Entities.DtoModels;
using Entities.DtoModels.BodyModels;
using Entities.DtoModels.QueryModels;
using Microsoft.AspNetCore.Http;

namespace Services.Contracts
{
    public interface IMachineService
	{
		Task CreateMachineAsync(MachineBodyDtoForCreate machineDtoC);

		Task<IEnumerable<MachineDto>> GetAllMachinesWithPagingAsync(
			PaginationQueryDto paginationParameters,
			HttpResponse response,
			bool trackChanges = false);

		Task<IEnumerable<MachineDto>> GetMachinesByConditionWithPagingAsync(
			MachineBodyDtoForDisplay machineDtoD,
			PaginationQueryDto pagingParameters,
			HttpResponse response);

		Task<IEnumerable<string>> GetSubCategoriesOfMainCategoryAsync(
			string mainCategoryName);

		Task UpdateMachineAsync(
			MachineQueryDtoForUpdate machineQueryDtoU,
            MachineBodyDtoForUpdate machineBodyDtoU);

		Task DeleteMachinesAsync(MachineQueryDtoForDelete machineQueryDto);
	}
}
