using DataAbstraction.Repositories;
using Domain.Entidades;

namespace Data.Repositories;

public abstract class BaseRepository<TEntity> : BaseLocalReadOnlyRepository<TEntity>, IRepository<TEntity>
where TEntity : ILocalEntity
{

  public BaseRepository(ILocalDataContext<TEntity> localDataContext)
  : base(localDataContext)
  {
  }


  public virtual Task AdicionarAsync(TEntity entidade)
  {
    localDataContext.Dados()[entidade.Id] = entidade;
    return Task.CompletedTask;
  }

  public virtual Task AtualizarAsync(TEntity entidade)
  {
    if (localDataContext.Dados().ContainsKey(entidade.Id))
    {
      localDataContext.Dados()[entidade.Id] = entidade;
    }
    return Task.CompletedTask;
  }

  public virtual Task<bool> DeletarAsync(Guid id)
  {
    return Task.FromResult(localDataContext.Dados().Remove(id, out _));
  }
}

public abstract class BaseLocalReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity>
where TEntity : ILocalEntity
{
  protected readonly ILocalDataContext<TEntity> localDataContext;

  public BaseLocalReadOnlyRepository(ILocalDataContext<TEntity> localDataContext)
  {
    this.localDataContext = localDataContext;
  }


  public virtual Task<TEntity?> ObterPorIdAsync(Guid id)
  {
    localDataContext.Dados().TryGetValue(id, out var cliente);
    return Task.FromResult(cliente);
  }

  public virtual Task<IEnumerable<TEntity>> ObterTodosAsync()
  {
    return Task.FromResult(localDataContext.Dados().Values.AsEnumerable());
  }
}

public abstract class BaseExternalReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity>
where TEntity : IExternalEntity
{
  private readonly IExternalDataContext<TEntity> externalDataContext;

  public BaseExternalReadOnlyRepository(IExternalDataContext<TEntity> externalDataContext)
  {
    this.externalDataContext = externalDataContext;
  }


  public virtual Task<TEntity?> ObterPorIdAsync(Guid id)
  {
    externalDataContext.Dados().TryGetValue(id, out var cliente);
    return Task.FromResult(cliente);
  }

  public virtual Task<IEnumerable<TEntity>> ObterTodosAsync()
  {
    return Task.FromResult(externalDataContext.Dados().Values.AsEnumerable());
  }
}


