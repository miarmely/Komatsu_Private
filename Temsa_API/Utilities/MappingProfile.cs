﻿using AutoMapper;
using Entities.DtoModels.UserDtos;
using Entities.ViewModels;

namespace Temsa_Api.Utilities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserView, UserDto>();

            #region userDtoForRegister to userDtoForCreate
            CreateMap<UserDtoForRegister, UserDtoForCreate>()
                .ForMember(
                    dst => dst.RoleNames,
                    opt => opt.MapFrom(r =>
                        new List<string> { "User" }));  // add default role as "User"
            #endregion
        }
    }
}
