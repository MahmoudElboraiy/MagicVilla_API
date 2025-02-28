using MagicVilla_API.Models;
using System.Linq.Expressions;

namespace MagicVilla_API.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task CreateAsync(T villa);
        Task RemoveAsync(T villa);
        Task SaveAsync();
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
        Task<T> GetAsync(Expression<Func<T, bool>>filter = null, bool tracked = true);
    }
}
