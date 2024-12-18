using StarWars.Domain.Interface;

namespace StarWars.Bl.Interface;

public interface IService<TEntity>: IRepository<TEntity> where TEntity : class;