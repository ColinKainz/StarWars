using Microsoft.EntityFrameworkCore;
using StarWars.Domain.Interface;
using StarWars.Model.Configuration;

namespace StarWars.Domain.Repository;

public abstract class ARepository<TEntity>: IRepository<TEntity> where TEntity : class
{
    private readonly StarWarsContext _context;
    private readonly DbSet<TEntity> _table;
    
    protected ARepository(StarWarsContext context) {
        _context = context;
        _table = context.Set<TEntity>();
    }
 
    public async Task<IEnumerable<TEntity>?> GetAllAsync()
    {
        return await _table.ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await _table.FindAsync(id);
    }

    public async Task<TEntity?> AddAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var keyProperty = _context.Model.FindEntityType(typeof(TEntity))!
            .FindPrimaryKey()!
            .Properties[0];
        var keyValue = keyProperty.PropertyInfo!.GetValue(entity);
        
        var existingEntity = await _table.FindAsync(keyValue);

        if (existingEntity != null)
            return null;
        
        await _table.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<TEntity>?> CreateRangeAsync(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var entityType = _context.Model.FindEntityType(typeof(TEntity));
        var keyProperty = entityType.FindPrimaryKey().Properties.First();

        var addedEntities = new List<TEntity>();

        foreach (var entity in entities)
        {
            var keyValue = keyProperty.PropertyInfo.GetValue(entity);

            var existingEntity = await _table.FindAsync(keyValue);

            if (existingEntity != null) continue;
            await _table.AddAsync(entity);
            addedEntities.Add(entity);
        }

        if (addedEntities.Count == 0)
        {
            return null;
        }

        await _context.SaveChangesAsync();

        return addedEntities;
    }


    public async Task<TEntity> UpSertAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        var keyProperty = _context.Model.FindEntityType(typeof(TEntity))!
            .FindPrimaryKey()!
            .Properties
            .First();
        var keyValue = keyProperty.PropertyInfo!.GetValue(entity);
        
        var existingEntity = await _table.FindAsync(keyValue);

        if (existingEntity == null)
            await _table.AddAsync(entity);
        else
            _context.Entry(existingEntity).CurrentValues.SetValues(entity);

        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<IEnumerable<TEntity>> UpSertRangeAsync(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        
        var entityType = _context.Model.FindEntityType(typeof(TEntity));
        var keyProperty = entityType!.FindPrimaryKey()!.Properties[0];

        foreach (var entity in entities)
        {
            var keyValue = keyProperty.PropertyInfo!.GetValue(entity);

            var existingEntity = await _table.FindAsync(keyValue);

            if (existingEntity == null)
                await _table.AddAsync(entity);
            else
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
        }

        await _context.SaveChangesAsync();

        return entities;
    }

    public async Task<TEntity?> DeleteAsyncById(int id)
    {
        ArgumentNullException.ThrowIfNull(id);
        
        var entity = await _table.FindAsync(id);
        if (entity is null) return null;
        
        _table.Remove(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity?> DeleteAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entityType = _context.Model.FindEntityType(typeof(TEntity));
        var keyProperty = entityType!.FindPrimaryKey()!.Properties[0];

        var keyValue = keyProperty.PropertyInfo!.GetValue(entity);
        var existingEntity = await _table.FindAsync(keyValue);

        if (existingEntity == null) return null;
        
        _table.Remove(existingEntity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<TEntity>?> DeleteRangeAsync(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var entityType = _context.Model.FindEntityType(typeof(TEntity));
        var keyProperty = entityType!.FindPrimaryKey()!.Properties[0];
        var deletedEntities = new List<TEntity>();

        foreach (var entity in entities)
        {
            var keyValue = keyProperty.PropertyInfo!.GetValue(entity);
            var existingEntity = await _table.FindAsync(keyValue);
            
            if (existingEntity == null) continue;
            
            deletedEntities.Add(existingEntity);
            _table.Remove(existingEntity);
        }
        await _context.SaveChangesAsync();

        return deletedEntities;
    }
    
    public async Task<IEnumerable<TEntity>?> GetPagedAsync(int page, int pageSize)
    {
        return await _table.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }
}