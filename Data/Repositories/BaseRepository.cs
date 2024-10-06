using DataAbstraction.Repositories;
using Domain.Entidades;

namespace Data.Repositories;

public abstract class BaseRepository<TEntity> : BaseLocalReadOnlyRepository<TEntity>, IRepository<TEntity>
where TEntity : ILocalEntity
{
  private readonly ILocalDataContext<TEntity> localDataContext;

  public BaseRepository(ILocalDataContext<TEntity> localDataContext)
  {
    this.localDataContext = localDataContext;
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
    return Task.FromResult(DadosParaTeste.Dados<TEntity>().TryRemove(id, out _));
  }
}

public abstract class BaseLocalReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity>
where TEntity : ILocalEntity
{
  public virtual Task<TEntity?> ObterPorIdAsync(Guid id)
  {
    DadosParaTeste.Dados<TEntity>().TryGetValue(id, out var cliente);
    return Task.FromResult(cliente);
  }

  public virtual Task<IEnumerable<TEntity>> ObterTodosAsync()
  {
    return Task.FromResult(DadosParaTeste.Dados<TEntity>().Values.AsEnumerable());
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


