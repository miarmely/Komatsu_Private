﻿using AutoMapper;
using Entities.DataModels;
using Entities.DtoModels;
using Entities.ErrorModels;
using Services.Contracts;

namespace Temsa.Utilities
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<User, UserDto>();
			CreateMap<UserDtoForRegister, User>();
			CreateMap<ErrorDetails, ErrorDto>();
		}
	}
}
