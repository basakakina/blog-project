using ApplicationCore.Entities.Abstract;
using ApplicationCore.Entities.Enums;
using DataAccess.DbContext;
using DataAccess.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccess.Services.Concrete
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly BlogDbContext _context;
        private readonly DbSet<T> _table;

        public BaseRepository(BlogDbContext context)
        {
            _context = context;
            _table = _context.Set<T>();
        }

        public async Task<bool> AddAsync(T entity)
        {
            
            entity.CreatedDate = entity.CreatedDate == default ? DateTime.Now : entity.CreatedDate;
            if (entity.Status == 0) entity.Status = Status.Active;

            await _table.AddAsync(entity);
            return await SaveAsync();
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            entity.UpdatedDate = DateTime.Now;
            entity.Status = Status.Modified;

            _table.Update(entity);
            return await SaveAsync();
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            // Soft delete
            entity.DeletedDate = DateTime.Now;
            entity.Status = Status.Passive;

            _table.Update(entity);
            return await SaveAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
            => await _table.AnyAsync(expression);

        public async Task<T> GetByIdAsync(Guid id)
            => await _table.FirstOrDefaultAsync(x => x.Status != Status.Passive && x.Id == id);

        public async Task<T> GetByDefaultAsync(Expression<Func<T, bool>> expression)
            => await _table.FirstOrDefaultAsync(expression);

        public async Task<List<T>> GetByDefaultsAsync(Expression<Func<T, bool>> expression)
            => await _table.Where(expression).ToListAsync();

        public async Task<List<TResult>> GetFilteredListAsync<TResult>(
            Expression<Func<T, TResult>> select,
            Expression<Func<T, bool>> where = null!,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null!,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> join = null!)
        {
            IQueryable<T> query = _table;

            if (join != null)
                query = join(query);

            if (where != null)
                query = query.Where(where);

            if (orderBy != null)
                return await orderBy(query).Select(select).ToListAsync();

            return await query.Select(select).ToListAsync();
        }

        private async Task<bool> SaveAsync()
            => await _context.SaveChangesAsync() > 0;
    }
}
