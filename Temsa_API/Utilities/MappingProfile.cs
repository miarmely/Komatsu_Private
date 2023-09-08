﻿using AutoMapper;
using Entities.DataModels;
using Entities.DtoModels;
using Entities.DtoModels.Machine;
using Entities.DtoModels.User;
using Entities.ErrorModels;

namespace Temsa.Utilities
{
    public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<User, UserDto>();
			CreateMap<UserDtoForRegisterWithoutRole, User>();
			CreateMap<ErrorDetails, ErrorDto>();
			CreateMap<MachineDtoForCreate, MachineDto>();
		}
	}
}
