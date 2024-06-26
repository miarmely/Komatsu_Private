﻿using Entities.DtoModels.FormDtos;
using Entities.QueryParameters;
using Microsoft.AspNetCore.Mvc;
using Presantation.Attributes;
using Services.Contracts;


namespace Presantation.Controllers
{
    [Route("api/services/[controller]")]
    [ApiController]
    public class FormController : ControllerBase
    {
        private readonly IServiceManager _manager;

        public FormController(IServiceManager manager) =>
            _manager = manager;


        [HttpPost("create/generalCommunication")]
        [Authorization("User,Kullanıcı")]
        public async Task<IActionResult> CreateGeneralCommunicationForm(
            [FromQuery] LanguageParams languageParams,
            [FromBody] GeneralCommFormDtoForCreate formDto)
        {
            var response = await _manager.FormService.CreateGenaralCommFormAsync(
                languageParams,
                formDto,
                HttpContext);

            return Ok(response);
        }


        [HttpPost("create/getOffer")]
        [Authorization("User,Kullanıcı")]
        public async Task<IActionResult> CreateGetOfferForm(
            [FromQuery] LanguageParams languageParams,
            [FromBody] GetOfferFormDtoForCreate formDto)
        {
            var response = await _manager.FormService
                .CreateGetOfferFormAsync(languageParams, formDto, HttpContext);

            return Ok(response);
        }


        [HttpPost("create/renting")]
        [Authorization("User,Kullanıcı")]
        public async Task<IActionResult> CreateRentingForm(
            [FromQuery] LanguageParams languageParams,
            [FromBody] RentingFormDtoForCreate formDto)
        {
			var response = await _manager.FormService
                .CreateRentingFormAsync(languageParams, formDto, HttpContext);

            return Ok(response);
        }


        [HttpGet("display/generalCommunication/all")]
        [Authorization]
        public async Task<IActionResult> DisplayAllGeneralCommForms(
            [FromQuery] FormParamsForDisplayAllGeneralCommForms formParams)
        {
            var formViews = await _manager.FormService
                .DisplayAllGeneralCommFormsAsync(formParams, HttpContext);

            return Ok(formViews);
        }


        [HttpGet("display/getOffer/all")]
        [Authorization]
        public async Task<IActionResult> DisplayAllGetOfferForms(
            [FromQuery] FormParamsForDisplayAllGetOfferForms formParams)
        {
            var formViews = await _manager.FormService
                .DisplayAllGetOfferFormsAsync(formParams, HttpContext);

            return Ok(formViews);
        }


        [HttpGet("display/renting/all")]
        [Authorization]
        public async Task<IActionResult> DisplayAllRentingForms(
            [FromQuery] FormParamsForDisplayAllRentingForms formParams)
        {
            var formViews = await _manager.FormService
                .DisplayAllRentingFormsAsync(formParams, HttpContext);

            return Ok(formViews);
        }


        [HttpGet("display/generalCommunication/oneUser")]
        [Authorization]
        public async Task<IActionResult> DisplayGeneralCommFormsOfOneUser(
            [FromQuery] FormParamsForDisplayGeneralCommFormsOfUser formParams)
        {
            var allForms = await _manager.FormService
                .DisplayGeneralCommFormsOfUserAsync(formParams, HttpContext);

            return Ok(allForms);
        }


        [HttpGet("display/getOffer/oneUser")]
        [Authorization]
        public async Task<IActionResult> DisplayGetOfferFormsOfUser(
            [FromQuery] FormParamsForDisplayGetOfferFormsOfUser formParams)
        {
            var allForms = await _manager.FormService
                .DisplayGetOfferFormsOfUserAsync(formParams, HttpContext);

            return Ok(allForms);
        }


        [HttpGet("display/renting/oneUser")]
        [Authorization]
        public async Task<IActionResult> DisplayRentingFormsOfUser(
            [FromQuery] FormParamsForDisplayRentingFormsOfUser formParams)
        {
            var allForms = await _manager.FormService
                 .DisplayRentingFormsOfUserAsync(formParams, HttpContext);

            return Ok(allForms);
        }


        [HttpPost("answer/generalCommunication")]
        [Authorization("Admin,Editor,Yönetici,Editör")]
        public async Task<IActionResult> AnswerTheGeneralCommForm(
            [FromQuery] FormParamsForAnswerTheGeneralCommForm formParams)
        {
            var answererInfos = await _manager.FormService.AnswerTheFormAsync(
                formParams,
                HttpContext);

            return Ok(answererInfos);
        }


        [HttpPost("answer/getOffer")]
        [Authorization("Admin,Editor,Yönetici,Editör")]
        public async Task<IActionResult> AnswerTheGetOfferForm(
            [FromQuery] FormParamsForAnswerTheGetOfferForm formParams)
        {
            var answererInfos = await _manager.FormService.AnswerTheFormAsync(
                formParams,
                HttpContext);

            return Ok(answererInfos);
        }


        [HttpPost("answer/renting")]
        [Authorization("Admin,Editor,Yönetici,Editör")]
        public async Task<IActionResult> AnswerTheRentingForm(
            [FromQuery] FormParamsForAnswerTheRentingForm formParams)
        {
            var answererInfos = await _manager.FormService.AnswerTheFormAsync(
                formParams,
                HttpContext);

            return Ok(answererInfos);
        }
    }
}