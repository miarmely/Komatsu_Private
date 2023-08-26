﻿using Entities.ConfigModels;
using Microsoft.EntityFrameworkCore;
using Repositories.Concretes;
using Repositories.Contracts;
using Repositories.EF;
using Services.Concretes;
using Services.Contracts;

namespace Temsa.Extensions
{
	public static class ServiceExtensions
	{
		public static void ConfigureRepositoryContext(this IServiceCollection services
			, IConfiguration configuration) =>
				services.AddDbContext<RepositoryContext>(options =>
					options.UseSqlServer(configuration
						.GetConnectionString("SqlServer")));

		public static void ConfigureRepositoryManager(this IServiceCollection services) =>
			services.AddScoped<IRepositoryManager, RepositoryManager>();
		
		public static void ConfigureServiceManager(this IServiceCollection services) =>
			services.AddScoped<IServiceManager, ServiceManager>();

		public static void ConfigureLoggerService(this IServiceCollection services) =>
			services.AddSingleton<ILoggerService, LoggerService>();
	}
}
