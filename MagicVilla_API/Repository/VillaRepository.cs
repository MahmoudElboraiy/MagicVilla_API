using MagicVilla_API.Data;
using MagicVilla_API.Models;
using MagicVilla_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_API.Repository
{
    public class VillaRepository : IVillaRepository
    {
        private readonly ApplicationDbContext _db;

        public VillaRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task Create(Villa villa)
        {
           await _db.Villas.AddAsync(villa);
           await Save();
        }

        public async Task<Villa> Get(Expression<Func<Villa, bool>> filter = null, bool tracked = true)
        {
            IQueryable<Villa> query = _db.Villas;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<Villa>> GetAll(Expression<Func<Villa, bool>> filter = null)
        {
           IQueryable<Villa> query = _db.Villas;
           if(filter != null)
            {
                query = query.Where(filter);
            }
            return await  query.ToListAsync();
        }

        public async Task Remove(Villa villa)
        {
             _db.Villas.Remove(villa);
            await Save();
        }

        public async Task Save()
        {
           await _db.SaveChangesAsync();
        }
    }
}
