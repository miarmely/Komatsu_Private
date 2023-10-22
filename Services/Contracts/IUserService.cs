﻿using Entities.DtoModels.UserDtos;
using Entities.QueryParameters;
using Microsoft.AspNetCore.Http;

namespace Services.Contracts
{
    public interface IUserService
	{
        Task<string> LoginAsync(string language, UserDtoForLogin userDto);
        Task RegisterAsync(string language, UserDtoForRegister userDto);
        Task CreateUserAsync(string language, UserDtoForCreate userDto);
        Task DeleteUsersByTelNoListAsync(UserDtoForDelete userDto);
        Task<IEnumerable<string>> GetAllRolesByLanguageAsync(string language);
        Task<IEnumerable<UserDto>> GetAllUsersWithPagingAsync(
            PaginationParameters pagingParameters,
            string language,
            HttpResponse response);
        Task UpdateUserByTelNoAsync(
            string language,
            string telNo,
            UserDtoForUpdate userDto);
    }
}
