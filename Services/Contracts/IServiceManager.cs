﻿namespace Services.Contracts
{
	public interface IServiceManager
	{
		IUserService UserService { get; }
		ICompanyService CompanyService { get; }
		IMailService MailService { get; }
		IMachineService MachineService { get; }
		IFileService FileService { get; }
		IRoleService RoleService { get; }
	}
}
