using System.Linq.Expressions;

namespace StarWars.Data;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filter = null, IEnumerable<string>? includes = null);
    Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>>? filter = null, IEnumerable<string>? includes = null);

    void Insert(TEntity entity);
    void Delete(object id);
    void Delete(TEntity entityToDelete);
    void Update(TEntity entityToUpdate);

}