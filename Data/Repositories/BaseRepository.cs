using DataAbstraction.Repositories;
using Domain.Entidades;

namespace Data.Repositories;

public abstract class BaseRepository<TEntity> : BaseLocalReadOnlyRepository<TEntity>, IRepository<TEntity>
where TEntity : ILocalEntity
{
  public virtual Task AdicionarAsync(TEntity entidade)
  {
    DadosParaTeste.Dados<TEntity>()[entidade.Id] = entidade;
    return Task.CompletedTask;
  }

  public virtual Task AtualizarAsync(TEntity entidade)
  {
    if (DadosParaTeste.Dados<TEntity>().ContainsKey(entidade.Id))
    {
      DadosParaTeste.Dados<TEntity>()[entidade.Id] = entidade;
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
  public virtual Task<TEntity?> ObterPorIdAsync(Guid id)
  {
    DadosParaTesteExternal.Dados<TEntity>().TryGetValue(id, out var cliente);
    return Task.FromResult(cliente);
  }

  public virtual Task<IEnumerable<TEntity>> ObterTodosAsync()
  {
    return Task.FromResult(DadosParaTesteExternal.Dados<TEntity>().Values.AsEnumerable());
  }
}


