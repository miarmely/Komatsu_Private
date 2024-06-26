﻿using Entities.DtoModels;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;

namespace Temsa_Api.Extensions
{
    public static class ErrorMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(
            this IServiceCollection services,
            WebApplication app)
        {
            app.UseExceptionHandler(configure =>
            {
                configure.Run(async context =>
                {
                    #region get context feature
                    var contextFeature = context.Features
                        .Get<IExceptionHandlerFeature>();
                    #endregion

                    #region when any error occured
                    if (contextFeature != null)
                    {
                        #region add default "contentType" and "StatusCode" to response
                        var exceptionMessage = contextFeature.Error.Message;

                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = 500;
                        #endregion

                        try
                        {
                            #region get errorDto (catch)
                            var errorDto = JsonSerializer
                                .Deserialize<ErrorDtoWithMessage>(exceptionMessage);

                            // if error message can be deserialize then this error is
                            // expected error. But when error message can't be
                            // desirealize then this error is unexpected error and not
                            // know where from throwed so this error should show on
                            // "catch" without desiralize 
                            #endregion

                            #region when error occured in "login"
                            var loginPathString = new PathString(
                                "/api/services/user/login");

                            // don't return the error details (for security)
                            if (context.Request.Path
                                .StartsWithSegments(loginPathString))
                            {
                                context.Response.StatusCode = 404;

                                await context.Response.CompleteAsync();
                                return;
                            }
                            #endregion

                            #region when error type is "format error"
                            if (errorDto.ErrorCode.StartsWith("FE-"))
                            {
                                #region get language from query
                                // get language
                                var language = context.Request.Query
                                    .FirstOrDefault(q => q.Key
                                        .ToLower()
                                        .Equals("language"))
                                    .Value;

                                // control the language whether valid
                                var validLanguages = new List<string> { "TR", "EN" };

                                language = validLanguages.Contains(language) ?
                                    language
                                    : "TR"; // when language not valid
                                #endregion

                                #region create new errorDto
                                // deserialize error message of format error
                                var errorMessageByLanguages = JsonSerializer
                                    .Deserialize(
                                        errorDto.ErrorMessage,
                                        typeof(IDictionary<string, string>))
                                    as IDictionary<string, string>;

                                // create new errorDto
                                errorDto = new ErrorDtoWithMessage
                                {
                                    StatusCode = errorDto.StatusCode,
                                    ErrorCode = errorDto.ErrorCode,
                                    ErrorDescription = errorDto.ErrorDescription,
                                    ErrorMessage = errorMessageByLanguages[language]
                                };
                                #endregion
                            }
                            #endregion

                            #region when expected error occured
                            context.Response.StatusCode = errorDto.StatusCode;

                            await context.Response.WriteAsJsonAsync(errorDto);
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            #region when throwed error from unexpected place
                            await context.Response.WriteAsJsonAsync(new
                            {
                                StatusCode = 500,
                                ErrorCode = "ISE",
                                ErrorDescription = "Internal Server Error",
                                ErrorMessage = "an error is occured on server"
                            });
                            #endregion
                        }
                    }
                    #endregion
                });
            });
        }
    }
}
