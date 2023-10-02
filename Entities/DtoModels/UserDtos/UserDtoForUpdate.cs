﻿namespace Entities.DtoModels.UserDtos
{
    public record UserDtoForUpdate
    {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? CompanyName { get; init; }
        public string? TelNo { get; init; }
        public string? Email { get; init; }
        public string? Password { get; init; }
        public List<string>? RoleNames { get; init; }
    }

    public record UserDtoForUpdateProcedure
    {
        public string TelNoForValidation { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? CompanyName { get; init; }
        public string? TelNo { get; init; }
        public string? Email { get; init; }
        public string? Password { get; init; }
        public string? RoleNames { get; init; }
    }
}