using System.Linq.Expressions;

namespace Persona_work_management.Repository.Generic
{
	public interface IRepository<T> where T : class
	{
		Task<IEnumerable<T>> GetAllAsync();
		Task<T?> GetByIdAsync(int id);
		Task AddAsync(T entity);
		Task UpdateAsync(T entity);
		Task DeleteAsync(T entity);
		Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
	}
}
