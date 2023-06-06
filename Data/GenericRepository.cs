using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace StarWars.Data;

public class GenericRepository<TEntity> where TEntity : class
{
    internal StarWarsContext _context;
    internal DbSet<TEntity> dbSet;

    public GenericRepository(StarWarsContext context)
    {
        _context = context;
        dbSet = context.Set<TEntity>();
    }

    private IQueryable<TEntity> addFilter(IQueryable<TEntity> query, Expression<Func<TEntity, bool>>? filter)
    {
        if (filter is not null)
        {
            query = query.Where(filter);
        }
        return query;
    }

    private IQueryable<TEntity> addOrderBy(IQueryable<TEntity> query, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
    {
        if (orderBy is not null)
        {
            query = orderBy(query);
        }
        return query;
    }

    private IQueryable<TEntity> addIncludes(IQueryable<TEntity> query, IEnumerable<string>? includes = null)
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

    public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            IEnumerable<string>? includes = null)
    {
        IQueryable<TEntity> query = dbSet;

        query = addFilter(query, filter);

        query = addIncludes(query, includes);

        query = addOrderBy(query, orderBy);

        return query.ToList();
    }

    public virtual TEntity? SingleOrDefault(
            Expression<Func<TEntity, bool>>? filter = null,
            IEnumerable<string>? includes = null)
    {
        IQueryable<TEntity> query = dbSet;

        query = addIncludes(query, includes);

        return filter is not null ? query.SingleOrDefault(filter) : query.SingleOrDefault();
    }

    public virtual void Insert(TEntity entity)
    {
        dbSet.Add(entity);
    }

    public virtual void Delete(object id)
    {
        TEntity entityToDelete = dbSet.Find(id);
        Delete(entityToDelete);
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