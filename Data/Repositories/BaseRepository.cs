using DataAbstraction.Repositories;
using Domain.Entidades;

namespace Data.Repositories;

public abstract class BaseRepository<TEntity> : IRepository<TEntity>
where TEntity : IEntity
{
  public virtual Task AdicionarAsync(TEntity entidade)
  {
    DbEmMemoria.Dados<TEntity>()[entidade.Id] = entidade;
    return Task.CompletedTask;
  }

  public virtual Task AtualizarAsync(TEntity entidade)
  {
    if (DbEmMemoria.Dados<TEntity>().ContainsKey(entidade.Id))
    {
      DbEmMemoria.Dados<TEntity>()[entidade.Id] = entidade;
    }
    return Task.CompletedTask;
  }

  public virtual Task<bool> DeletarAsync(Guid id)
  {
    return Task.FromResult(DbEmMemoria.Dados<TEntity>().TryRemove(id, out _));
  }

  public virtual Task<TEntity?> ObterPorIdAsync(Guid id)
  {
    DbEmMemoria.Dados<TEntity>().TryGetValue(id, out var cliente);
    return Task.FromResult(cliente);
  }

  public virtual Task<IEnumerable<TEntity>> ObterTodosAsync()
  {
    return Task.FromResult(DbEmMemoria.Dados<TEntity>().Values.AsEnumerable());
  }
}
