﻿using System.Linq.Expressions;

namespace Repositories.Contracts
{
	public interface IRepositoryBase<T>
	{
		int Count { get; }
		void Create(T entity);
		IQueryable<T> FindAll(bool trackChanges);
		IQueryable<T> FindWithCondition(Expression<Func<T, bool>> expression
			, bool trackChanges);
		void Update(T entity);
		void Delete(T entity);
	}
}
