﻿using Entities.ConfigModels.Contracts;
using Repositories.Contracts;
using Repositories.MiarLibrary.Concretes;
using Repositories.MiarLibrary.Contracts;

namespace Repositories.Concretes
{
	public class RepositoryManager : IRepositoryManager
	{
		private readonly Lazy<IUserRepository> _userRepository;
		private readonly Lazy<IMachineRepository> _machineRepository;
		private readonly Lazy<IFileRepository> _fileRepository;
		private readonly Lazy<IFormRepository> _formRepository;
		private readonly Lazy<IMachineCategoryRepository> _machineCategoryRepository;
		private readonly Lazy<IPasswordRepository> _passwordRepository;

 		public IUserRepository UserRepository => _userRepository.Value;
		public IMachineRepository MachineRepository => _machineRepository.Value;
		public IFileRepository FileRepository => _fileRepository.Value;
		public IFormRepository FormRepository => _formRepository.Value;
		public IMachineCategoryRepository MachineCategoryRepository =>
			_machineCategoryRepository.Value;
		public IPasswordRepository PasswordRepository => _passwordRepository.Value;

		public RepositoryManager(
			RepositoryContext context,
			IConfigManager configs)
		{
			_userRepository = new Lazy<IUserRepository>(() =>
				new UserRepository(context, configs));
			_machineRepository = new Lazy<IMachineRepository>(() => 
				new MachineRepository(context, configs));
			_fileRepository = new Lazy<IFileRepository>(() =>
				new FileRepository(context, configs));
			_formRepository = new Lazy<IFormRepository>(() =>
				new FormRepository(context, configs));
			_machineCategoryRepository = new Lazy<IMachineCategoryRepository>(() => 
				new MachineCategoryRepository(context, configs));
			_passwordRepository = new Lazy<IPasswordRepository>(() =>
				new PasswordRepository(context, configs));
		}
	}
}