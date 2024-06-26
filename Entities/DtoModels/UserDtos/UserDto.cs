﻿namespace Entities.DtoModels.UserDtos
{
    public record UserDto
    {
        public Guid? UserId { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? CompanyName { get; init; }
        public string? TelNo { get; init; }
        public string? Email { get; init; }
        public ICollection<string>? RoleNames { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
