﻿using Dapper;
using Entities.ConfigModels.Contracts;
using Entities.DtoModels.FormDtos;
using Entities.Enums;
using Entities.Exceptions;
using Entities.QueryParameters;
using Entities.ViewModels;
using Entities.ViewModels.FormViews;
using Microsoft.AspNetCore.Http;
using Repositories;
using Repositories.Contracts;
using Services.Contracts;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;


namespace Services.Concretes
{
	public partial class FormService : IFormService
	{

		private readonly IRepositoryManager _manager;
		private readonly IConfigManager _configs;

		public FormService(IRepositoryManager manager)
		{
			_manager = manager;
			_configs = _manager.FormRepository.Configs;
		}

		private async Task<JwtSecurityToken> GetTokenFromHttpContextAsync(
			HttpContext httpContext)
		{
			#region get token in str from http context
			var jwtTokenInStr = httpContext.Request.Headers.Authorization
				.ToString()
				.Replace("Bearer ", "");  // remove Bearer tag
			#endregion

			return new JwtSecurityToken(jwtTokenInStr);
		}

		private async Task<DynamicParameters> GetDynamicParametersWithUserIdAsync(
			HttpContext httpContext)
		{
			#region get user id from token
			var jwtToken = await GetTokenFromHttpContextAsync(httpContext);

			var userIdInStr = jwtToken.Claims
				.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))
				.Value;
			#endregion

			#region set parameters
			var parameters = new DynamicParameters();

			parameters.Add(
				"UserId",
				new Guid(userIdInStr),
				DbType.Guid);
			#endregion

			return parameters;
		}

		private async Task<PagingList<TPagingList>> GetPagingListOfFormViewAsync<TPagingList>(
			int pageNumber,
			int pageSize,
			string headerKey,
			HttpContext httpContext,
			FormTypes? formType = null,
			DynamicParameters? dynamicParameters = null,
			IEnumerable<TPagingList>? formView = null)
			where TPagingList : class
		{
			#region get formView if wasn't add to parameter
			if (formView == null)
				formView = formType switch
				{
					#region get general communication formView
					FormTypes.GeneralCommunication => await _manager.FormRepository
						.GetGeneralCommFormsOfOneUserAsync<TPagingList>(dynamicParameters),
					#endregion

					#region get get offer formView
					FormTypes.GetOffer => await _manager.FormRepository
						.GetGetOfferFormsOfOneUserAsync<TPagingList>(dynamicParameters),
					#endregion

					#region get renting formView
					FormTypes.Renting => await _manager.FormRepository
						.GetRentingFormsOfOneUserAsync<TPagingList>(dynamicParameters)
					#endregion
				};
			#endregion

			#region get paging list of formView
			var pagingList = await PagingList<TPagingList>
				.ToPagingListAsync(
					formView,
					formView.Count(),
					pageNumber,
					pageSize,
					headerKey,
					httpContext);
			#endregion

			return pagingList;
		}
	}

	public partial class FormService // main services
	{
		public async Task CreateGenaralCommFormAsync(
			GeneralCommFormDtoForCreate formDto,
			HttpContext httpContext)
		{
			#region set parameters
			var parameters = await GetDynamicParametersWithUserIdAsync(httpContext);

			parameters.AddDynamicParams(formDto);
			#endregion

			await _manager.UserRepository
				.CreateGeneralCommFormAsync(parameters);
		}

		public async Task CreateGetOfferFormAsync(
			LanguageParams languageParams,
			GetOfferFormDtoForCreate formDto,
			HttpContext httpContext)
		{
			#region set parameters
			var parameters = await GetDynamicParametersWithUserIdAsync(httpContext);

			parameters.AddDynamicParams(formDto);
			parameters.Add("Language", languageParams.Language, DbType.String);
			#endregion

			#region create form (throw)
			var errorDto = await _manager.UserRepository
				.CreateGetOfferFormAsync(parameters);

			if (errorDto != null)
				throw new ErrorWithCodeException(errorDto);
			#endregion
		}

		public async Task CreateRentingFormAsync(
			LanguageParams languageParams,
			RentingFormDtoForCreate formDto,
			HttpContext httpContext)
		{
			#region set parameters
			var parameters = await GetDynamicParametersWithUserIdAsync(httpContext);

			parameters.AddDynamicParams(formDto);
			parameters.Add("Language", languageParams.Language, DbType.String);
			#endregion

			#region create form (throw)
			var errorDto = await _manager.UserRepository
				.CreateRentingFormAsync(parameters);

			if (errorDto != null)
				throw new ErrorWithCodeException(errorDto);
			#endregion
		}

		public async Task<object> GetAllFormTypesOfOneUserAsync(
			FormParamsForGetAllFormsOfOneUser formParams,
			HttpContext httpContext)
		{
			#region set parameters and sql command
			var parameters = new DynamicParameters(formParams);
			var sqlCommand = $@" 
					-- get `general communication` forms
                    EXEC {_configs.DbSettings.ProcedureNames
							.User_Form_GeneralCommunication_GetAllOfOneUserByUserId}
                        @UserId,
                        @PageNumber,
                        @PageSize,
                        @GetAnsweredForms;
                    
					-- get `get offer` forms
                    EXEC {_configs.DbSettings.ProcedureNames
							.User_Form_GetOffer_GetAllOfOneUserByUserId}
                        @Language,
                        @UserId,
                        @PageNumber,
                        @PageSize,
                        @GetAnsweredForms;

					-- get `renting`forms
                    EXEC {_configs.DbSettings.ProcedureNames
							.User_Form_Renting_GetAllOfOneUserByUserId}
                        @Language,
                        @UserId,
                        @PageNumber,
                        @PageSize,
                        @GetAnsweredForms;";
			#endregion

			#region get all forms of one user in paging list
			object pagingList = formParams.GetAnsweredForms switch
			{
				#region get "answered" forms
				true => await _manager.UserRepository
					.GetAllFormsOfUserAsync(
						sqlCommand,
						parameters,
						async (multiQuery) =>
						{
							#region get "answered" all forms of one user
							var answeredGCFormViews = await multiQuery
								.ReadAsync<AnsweredGeneralCommFormViewForOneUser>();

							var answeredGOFormViews = await multiQuery
								.ReadAsync<AnsweredGetOfferFormViewForOneUser>();

							var answeredRFormViews = await multiQuery
								.ReadAsync<AnsweredRentingFormViewForOneUser>();
							#endregion

							#region save paging infos of forms to header
							var answeredGCFormPagingList = await GetPagingListOfFormViewAsync(
								formParams.PageNumber,
								formParams.PageSize,
								"Form-Answered-GeneralCommunication",
								httpContext,
								formView: answeredGCFormViews);

							var answeredGOFormPagingList = await GetPagingListOfFormViewAsync(
								formParams.PageNumber,
								formParams.PageSize,
								"Form-Answered-GetOffer",
								httpContext,
								formView: answeredGOFormViews);

							var answeredRFormPagingList = await GetPagingListOfFormViewAsync(
								formParams.PageNumber,
								formParams.PageSize,
								"Form-Answered-Renting",
								httpContext,
								formView: answeredRFormViews);
							#endregion

							return new AnsweredFormViewForOneUser()
							{
								GeneralCommunicationForms =
									answeredGCFormPagingList,
								GetOfferForms = answeredGOFormPagingList,
								RentingForms = answeredRFormPagingList

							};
						}),
				#endregion

				#region get "unanswered" forms
				false => await _manager.UserRepository
					.GetAllFormsOfUserAsync(
						sqlCommand,
						parameters,
						async (multiQuery) =>
						{
							#region get "unanswered" all forms of user
							var unansweredGCFormViews = await multiQuery
								.ReadAsync<UnansweredGeneralCommFormViewForOneUser>();

							var unansweredGOFormViews = await multiQuery
								.ReadAsync<UnansweredGetOfferFormViewForOneUser>();

							var unansweredRFormViews = await multiQuery
								.ReadAsync<UnansweredRentingFormViewForOneUser>();
							#endregion

							#region save paging infos of forms to header
							var unansweredGCFormPagingList = await
								GetPagingListOfFormViewAsync(
									formParams.PageNumber,
									formParams.PageSize,
									"Form-Unanswered-GeneralCommunication",
									httpContext,
									formView: unansweredGCFormViews);

							var unansweredGOFormPagingList = await
								GetPagingListOfFormViewAsync(
									formParams.PageNumber,
									formParams.PageSize,
									"Form-Unanswered-GetOffer",
									httpContext,
									formView: unansweredGOFormViews);

							var unansweredRFormPagingList = await
								GetPagingListOfFormViewAsync(
									formParams.PageNumber,
									formParams.PageSize,
									"Form-Unanswered-Renting",
									httpContext,
									formView: unansweredRFormViews);
							#endregion

							return new UnansweredFormViewForOneUser()
							{
								GeneralCommunicationForms =
									unansweredGCFormPagingList,
								GetOfferForms = unansweredGOFormPagingList,
								RentingForms = unansweredRFormPagingList
							};
						}),
				#endregion

				#region when "answered" and "unanswered" forms is wanted
				null => await _manager.UserRepository
					.GetAllFormsOfUserAsync(
						sqlCommand,
						parameters,
						async (multiQuery) =>
						{
							#region get "unanswered" all forms of user
							var unansweredGCFormViews = await multiQuery
								.ReadAsync<UnansweredGeneralCommFormViewForOneUser>();

							var unansweredGOFormViews = await multiQuery
								.ReadAsync<UnansweredGetOfferFormViewForOneUser>();

							var unansweredRFormViews = await multiQuery
								.ReadAsync<UnansweredRentingFormViewForOneUser>();
							#endregion

							#region save paging infos of forms to header
							var unansweredGCFormPagingList = await GetPagingListOfFormViewAsync(
								formParams.PageNumber,
								formParams.PageSize,
								"Form-Unanswered-GeneralCommunication",
								httpContext,
								formView: unansweredGCFormViews);

							var unansweredGOFormPagingList = await GetPagingListOfFormViewAsync(
								formParams.PageNumber,
								formParams.PageSize,
								"Form-Unanswered-GetOffer",
								httpContext,
								formView: unansweredGOFormViews);

							var unansweredRFormPagingList = await GetPagingListOfFormViewAsync(
								formParams.PageNumber,
								formParams.PageSize,
								"Form-Unanswered-Renting",
								httpContext,
								formView: unansweredRFormViews);
							#endregion

							return new UnansweredFormViewForOneUser()
							{
								GeneralCommunicationForms =
									unansweredGCFormPagingList,
								GetOfferForms = unansweredGOFormPagingList,
								RentingForms = unansweredRFormPagingList
							};
						}),
				#endregion
			};
			#endregion

			return pagingList;
		}

		public async Task<object> GetGeneralCommFormsOfOneUserAsync(
			FormParamsForGetGeneralCommFormsOfOneUser formParams,
			HttpContext httpContext)
		{
			#region get "general comm." forms belong to one user in paging List
			var parameters = new DynamicParameters(formParams);

			object pagingList = formParams.GetAnsweredForms switch
			{
				#region when "answered" forms is wanting
				true => await GetPagingListOfFormViewAsync
					<AnsweredGeneralCommFormViewForOneUser>(
						formParams.PageNumber,
						formParams.PageSize,
						"Form-Answered-GeneralCommunication",
						httpContext,
						FormTypes.GeneralCommunication,
						parameters),
				#endregion

				#region when "unanswered" forms is wanting
				false => await GetPagingListOfFormViewAsync
					<UnansweredGeneralCommFormViewForOneUser>(
						formParams.PageNumber,
						formParams.PageSize,
						"Form-Unanswered-GeneralCommunication",
						httpContext,
						FormTypes.GeneralCommunication,
						parameters),
				#endregion

				#region when "answered" and "unanswered" forms is wanting
				null => await GetPagingListOfFormViewAsync
					<AllGeneralCommFormViewForOneUser>(
						formParams.PageNumber,
						formParams.PageSize,
						"Form-All-GeneralCommunication",
						httpContext,
						FormTypes.GeneralCommunication,
						parameters),
				#endregion
			};
			#endregion

			return pagingList;
		}

		public async Task<object> GetGetOfferFormsOfOneUserAsync(
			FormParamsForGetGetOfferFormsOfOneUser formParams,
			HttpContext httpContext)
		{
			#region get "get offer" forms belong to one user in paging List
			var parameters = new DynamicParameters(formParams);

			object pagingList = formParams.GetAnsweredForms switch
			{
				#region when "answered" forms is wanting
				true => await GetPagingListOfFormViewAsync
					<AnsweredGetOfferFormViewForOneUser>(
						formParams.PageNumber,
						formParams.PageSize,
						"Form-Answered-GetOffer",
						httpContext,
						FormTypes.GetOffer,
						parameters),
				#endregion

				#region when "unanswered" forms is wanting
				false => await GetPagingListOfFormViewAsync
					<UnansweredGetOfferFormViewForOneUser>(
						formParams.PageNumber,
						formParams.PageSize,
						"Form-Unanswered-GetOffer",
						httpContext,
						FormTypes.GetOffer,
						parameters),
				#endregion

				#region when "answered" and "unanswered" forms is wanting
				null => await GetPagingListOfFormViewAsync
					<AllGetOfferFormViewForOneUser>(
						formParams.PageNumber,
						formParams.PageSize,
						"Form-All-GetOffer",
						httpContext,
						FormTypes.GetOffer,
						parameters),
				#endregion
			};
			#endregion

			return pagingList;
		}

		public async Task<object> GetRentingFormsOfOneUserAsync(
			FormParamsForGetRentingFormsOfOneUser formParams,
			HttpContext httpContext)
		{
			#region get "renting" forms belong to one user in paging List
			var parameters = new DynamicParameters(formParams);

			object pagingList = formParams.GetAnsweredForms switch
			{
				#region when "answered" forms is wanting
				true => await GetPagingListOfFormViewAsync
					<AnsweredRentingFormViewForOneUser>(
						formParams.PageNumber,
						formParams.PageSize,
						"Form-Answered-Renting",
						httpContext,
						FormTypes.Renting,
						parameters),
				#endregion

				#region when "unanswered" forms is wanting
				false => await GetPagingListOfFormViewAsync
					<UnansweredRentingFormViewForOneUser>(
						formParams.PageNumber,
						formParams.PageSize,
						"Form-Unanswered-Renting",
						httpContext,
						FormTypes.Renting,
						parameters),
				#endregion

				#region when "answered" and "unanswered" forms is wanting
				null => await GetPagingListOfFormViewAsync
					<AllRentingFormViewForOneUser>(
						formParams.PageNumber,
						formParams.PageSize,
						"Form-All-Renting",
						httpContext,
						FormTypes.Renting,
						parameters),
				#endregion
			};
			#endregion

			return pagingList;
		}
	}
}
