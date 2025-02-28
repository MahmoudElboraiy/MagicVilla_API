using MagicVilla_API.Models;
using System.Linq.Expressions;

namespace MagicVilla_API.Repository.IRepository
{
    public interface IVillaRepository
    {
        Task Create(Villa villa);
        Task Remove(Villa villa);
        Task Save();
        Task<List<Villa>> GetAll(Expression<Func<Villa>> filter = null);
        Task<Villa> Get(Expression<Func<Villa>> filter = null,bool tarcked =true);
    }
}
