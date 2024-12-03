﻿using Microsoft.EntityFrameworkCore;
using Persona_work_management.DAL;
using System.Linq.Expressions;

namespace Persona_work_management.Repository.Generic
{
	public class Repository<T> : IRepository<T> where T : class
	{
		protected readonly ManagementDbContext _context;
		protected readonly DbSet<T> _dbSet;

		public Repository(ManagementDbContext context)
		{
			_context = context;
			_dbSet = context.Set<T>();
		}

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task<T?> GetByIdAsync(int id)
		{
			return await _dbSet.FindAsync(id);
		}

		public async Task AddAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
		}

		public async Task UpdateAsync(T entity)
		{
			_dbSet.Update(entity);
		}

		public async Task DeleteAsync(T entity)
		{
			_dbSet.Remove(entity);
		}

		public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
		{
			return await _dbSet.Where(predicate).ToListAsync();
		}
	}
}
