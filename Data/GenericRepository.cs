using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace StarWars.Data;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    internal StarWarsContext _context;
    internal DbSet<TEntity> dbSet;

    public GenericRepository(StarWarsContext context)
    {
        _context = context;
        dbSet = context.Set<TEntity>();
    }

    protected IQueryable<TEntity> addFilter(IQueryable<TEntity> query, Expression<Func<TEntity, bool>>? filter)
    {
        if (filter is not null)
        {
            query = query.Where(filter);
        }
        return query;
    }

    protected IQueryable<TEntity> addIncludes(IQueryable<TEntity> query, IEnumerable<string>? includes)
    {
        if (includes is not null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return query;
    }

    public virtual async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            IEnumerable<string>? includes = null)
    {
        IQueryable<TEntity> query = dbSet;

        query = addFilter(query, filter);

        query = addIncludes(query, includes);
        
        return await query.ToListAsync();
    }

    public virtual async Task<TEntity?> SingleOrDefaultAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            IEnumerable<string>? includes = null)
    {
        IQueryable<TEntity> query = dbSet;

        query = addFilter(query, filter);

        query = addIncludes(query, includes);

        return await query.SingleOrDefaultAsync();
    }

    public virtual void Insert(TEntity entity)
    {
        dbSet.Add(entity);
    }

    public virtual void Delete(object id)
    {
        TEntity? entityToDelete = dbSet.Find(id);
        if(entityToDelete is not null)
        {
            Delete(entityToDelete);
        }
    }

    public virtual void Delete(TEntity entityToDelete)
    {
        if (_context.Entry(entityToDelete).State is EntityState.Detached)
        {
            dbSet.Attach(entityToDelete);
        }
        dbSet.Remove(entityToDelete);
    }

    public virtual void Update(TEntity entityToUpdate)
    {
        dbSet.Attach(entityToUpdate);
        _context.Entry(entityToUpdate).State = EntityState.Modified;
    }
}