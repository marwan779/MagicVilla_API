using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MagicVilla_VillaAPI.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            this._dbSet = _context.Set<T>();
        }


        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? Filter = null, string? IncludeProperties = null, int PageSize = 0, int PageNumber = 1)
        {
            IQueryable<T> Result = _dbSet;

            if(Filter != null)
            {
                Result = Result.Where(Filter);
            }
            if (PageSize > 0)
            {
                if (PageSize > 100)
                {
                    PageSize = 100;
                }
                //skip0.take(5)
                //page number- 2     || page size -5
                //skip(5*(1)) take(5)
                Result = Result.Skip(PageSize * (PageNumber - 1)).Take(PageSize);
            }
            if(IncludeProperties != null)
            {
                foreach (var includeProp in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Result = Result.Include(includeProp);
                }
            }

            return await Result.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>>? Filter = null, string? IncludeProperties = null, bool Tracked = true)
        {
            IQueryable<T> Result = _dbSet;

            if (Filter != null)
            {
                Result = Result.Where(Filter);
            }
            if (IncludeProperties != null)
            {
                foreach (var includeProp in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Result = Result.Include(includeProp);
                }
            }

            return await Result.FirstOrDefaultAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            _context.Remove(entity);
            await SaveAsync(); 
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
