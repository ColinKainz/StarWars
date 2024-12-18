using StarWars.Bl.Interface;
using StarWars.Domain.Interface;

namespace StarWars.Bl.Services;

public abstract class AService<TEntity>: IService<TEntity> where TEntity : class
{
    private readonly IRepository<TEntity> _repository;

    public AService(IRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TEntity>?> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<TEntity?> AddAsync(TEntity entity)
    {
        return await _repository.AddAsync(entity);
    }

    public async Task<IEnumerable<TEntity>?> CreateRangeAsync(IEnumerable<TEntity> entities)
    {
        return await _repository.CreateRangeAsync(entities);
    }

    public async Task<TEntity> UpSertAsync(TEntity entity)
    {
        return await _repository.UpSertAsync(entity);
    }

    public async Task<IEnumerable<TEntity>> UpSertRangeAsync(IEnumerable<TEntity> entities)
    {
        return await _repository.UpSertRangeAsync(entities);
    }

    public async Task<TEntity?> DeleteAsyncById(int id)
    {
        return await _repository.DeleteAsyncById(id);
    }

    public async Task<TEntity?> DeleteAsync(TEntity entity)
    {
        return await _repository.DeleteAsync(entity);
    }

    public async Task<IEnumerable<TEntity>?> DeleteRangeAsync(IEnumerable<TEntity> entities)
    {
        return await _repository.DeleteRangeAsync(entities);
    }

    public async Task<IEnumerable<TEntity>?> GetPagedAsync(int page, int pageSize)
    {
        return await _repository.GetPagedAsync(page, pageSize);
    }
}