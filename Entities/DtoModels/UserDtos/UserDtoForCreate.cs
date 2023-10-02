﻿namespace Entities.DtoModels.UserDtos
{
    public record UserDtoForCreate
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string CompanyName { get; init; }
        public string TelNo { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
        public List<string>? RoleNames { get; init; }
    }

    public record UserDtoForCreateProcedure
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string CompanyName { get; init; }
        public string TelNo { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
        public string RoleNames { get; init; }
    }
}