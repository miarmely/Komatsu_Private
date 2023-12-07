﻿using Entities.DtoModels.MachineDtos;
using Entities.QueryParameters;
using Microsoft.AspNetCore.Mvc;
using Presantation.Attributes;
using Services.Contracts;
using System.ComponentModel.DataAnnotations;


namespace Presantation.Controllers
{
	[Route("api/services/[Controller]")]
	[ApiController]
	public class MachineController : ControllerBase
	{
		private readonly IServiceManager _manager;

		public MachineController(IServiceManager services) =>
			_manager = services;


		[HttpPost("create")]
		[Authorization("Admin,Yönetici,Editor,Editör")]
		[ValidationNullArguments]
		public async Task<IActionResult> CreateMachine(
			[FromQuery] MachineParamsForCreate machineParams,
			[FromBody] MachineDtoForCreate machineDto)
		{
			await _manager.MachineService
				.CreateMachineAsync(machineParams, machineDto);

			return NoContent();
		}


		[HttpGet("display/all")]
		[Authorization("Admin,Yönetici,Editor,Editör,User,Kullanıcı")]
		public async Task<IActionResult> GetAllMachines(
			[FromQuery] LanguageParam languageParam,
			[FromQuery] PaginationParameters pagingParameters)
		{
			var machines = await _manager.MachineService.GetAllMachinesAsync(
				languageParam.Language, 
				pagingParameters, 
				Response);

			return Ok(machines);
		}


		[HttpGet("display/condition")]
		[Authorization("User,Kullanıcı,Editor,Editör,Admin,Yönetici")]
		public async Task<IActionResult> GetMachinesByCondition(
			[FromQuery] PaginationParameters pagingParameters,
			[FromQuery] MachineDtoForDisplay machineDto)
		{
			var machines = await _manager.MachineService.GetMachinesByConditionAsync(
				machineDto.Language,
				pagingParameters,
				machineDto,
				Response);

			return Ok(machines);
		}


		[HttpGet("display/mainCategory")]
		[Authorization("User,Kullanıcı,Editor,Editör,Admin,Yönetici")]
		public async Task<IActionResult> GetMainCategoryNamesByLanguage(
		   [FromQuery] LanguageParam languageParam)
		{
			var mainCategoryNames = await _manager.MachineService
				.GetMainCategoryNamesByLanguageAsync(languageParam.Language);

			return Ok(mainCategoryNames);
		}


		[HttpGet("display/subCategory")]
		[Authorization("User,Kullanıcı,Editor,Editör,Admin,Yönetici")]
		public async Task<IActionResult> GetSubCategoryNamesOfMainCategoryByLanguage(
			[FromQuery] MachineParamsForDisplaySubCategoryNames machineParams)
		{
			var subCategoryNames = await _manager.MachineService
				.GetSubCategoryNamesOfMainCategoryByLanguageAsync(machineParams);

			return Ok(subCategoryNames);
		}


		[HttpGet("display/handStatus")]
		[Authorization("Admin,Yönetici,Editor,Editör,User,Kullanıcı")]
		public async Task<IActionResult> GetAllHandStatusByLanguage(
			[FromQuery] LanguageParam languageParam)
		{
			var handStatuses = await _manager.MachineService
				.GetAllHandStatusByLanguageAsync(languageParam.Language);

			return Ok(handStatuses);
		}


		[HttpGet("display/language")]
		[Authorization("User,Kullanıcı,Editor,Editör,Admin,Yönetici")]
		public async Task<IActionResult> GetAllLanguages()
		{
			var languages = await _manager.MachineService
				.GetAllLanguagesAsync();

			return Ok(languages);
		}


		[HttpPut("update")]
		[Authorization("Admin,Yönetici,Editor,Editör")]
		[ValidationNullArguments]
		public async Task<IActionResult> UpdateMachine(
			[FromQuery][Required] MachineParamsForUpdate machineParams,
			[FromBody] MachineDtoForUpdate machineDto)
		{
			await _manager.MachineService
				.UpdateMachineAsync(machineParams, machineDto);

			return NoContent();
		}


		[HttpDelete("delete")]
		[Authorization("Admin,Yönetici")]
		[ValidationNullArguments]
		public async Task<IActionResult> DeleteMachines(
			[FromQuery] MachineParamsForDelete machineParams,
			[FromBody] IEnumerable<MachineDtoForDelete> machineDtos)
		{
			await _manager.MachineService.DeleteMachineAsync(
				machineParams,
				machineDtos);	

			return NoContent();
		}
	}
}
