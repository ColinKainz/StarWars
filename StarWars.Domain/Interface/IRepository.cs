namespace StarWars.Domain.Interface;

public interface IRepository<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>?> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity?> AddAsync(TEntity entity);
    Task<IEnumerable<TEntity>?> CreateRangeAsync(IEnumerable<TEntity> entities);
    Task<TEntity> UpSertAsync(TEntity entity);
    Task<IEnumerable<TEntity>> UpSertRangeAsync(IEnumerable<TEntity> entities);
    Task<TEntity?> DeleteAsyncById(int id);
    Task<TEntity?> DeleteAsync(TEntity entity);
    Task<IEnumerable<TEntity>?> DeleteRangeAsync(IEnumerable<TEntity> ids);
    Task<IEnumerable<TEntity>?> GetPagedAsync(int page, int pageSize);
}