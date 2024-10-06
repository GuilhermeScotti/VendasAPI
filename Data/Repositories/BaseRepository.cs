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
    _localDataContext.Dados()[entidade.Id] = entidade;
    return Task.CompletedTask;
  }

  public virtual Task AtualizarAsync(TEntity entidade)
  {
    if (_localDataContext.Dados().ContainsKey(entidade.Id))
    {
      _localDataContext.Dados()[entidade.Id] = entidade;
    }
    return Task.CompletedTask;
  }

  public virtual Task<bool> DeletarAsync(Guid id)
  {
    return Task.FromResult(_localDataContext.Dados().Remove(id, out _));
  }
}

public abstract class BaseLocalReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity>
where TEntity : ILocalEntity
{
  protected readonly ILocalDataContext<TEntity> _localDataContext;

  public BaseLocalReadOnlyRepository(ILocalDataContext<TEntity> localDataContext)
  {
    _localDataContext = localDataContext;
  }


  public virtual Task<TEntity?> ObterPorIdAsync(Guid id)
  {
    _localDataContext.Dados().TryGetValue(id, out var cliente);
    return Task.FromResult(cliente);
  }

  public virtual Task<IEnumerable<TEntity>> ObterTodosAsync()
  {
    return Task.FromResult(_localDataContext.Dados().Values.AsEnumerable());
  }
}

public abstract class BaseExternalReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity>
where TEntity : IExternalEntity
{
  private readonly IExternalDataContext<TEntity> _externalDataContext;

  public BaseExternalReadOnlyRepository(IExternalDataContext<TEntity> externalDataContext)
  {
    _externalDataContext = externalDataContext;
  }


  public virtual Task<TEntity?> ObterPorIdAsync(Guid id)
  {
    _externalDataContext.Dados().TryGetValue(id, out var cliente);
    return Task.FromResult(cliente);
  }

  public virtual Task<IEnumerable<TEntity>> ObterTodosAsync()
  {
    return Task.FromResult(_externalDataContext.Dados().Values.AsEnumerable());
  }
}


