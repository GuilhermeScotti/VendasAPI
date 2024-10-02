using DataAbstraction.Repositories;
using Domain.Entidades;

namespace Data.Repositories;

public abstract class BaseRepository<TEntity> : IRepository<TEntity>
where TEntity : IEntity
{
  public Task AdicionarAsync(TEntity entidade)
  {
    DbEmMemoria.Dados<TEntity>()[entidade.Id] = entidade;
    return Task.CompletedTask;
  }

  public Task AtualizarAsync(TEntity entidade)
  {
    if (DbEmMemoria.Dados<TEntity>().ContainsKey(entidade.Id))
    {
      DbEmMemoria.Dados<TEntity>()[entidade.Id] = entidade;
    }
    return Task.CompletedTask;
  }

  public Task<bool> DeletarAsync(Guid id)
  {
    return Task.FromResult(DbEmMemoria.Clientes.TryRemove(id, out _));
  }

  public Task<TEntity?> ObterPorIdAsync(Guid id)
  {
    DbEmMemoria.Dados<TEntity>().TryGetValue(id, out var cliente);
    return Task.FromResult(cliente);
  }

  public Task<IEnumerable<TEntity>> ObterTodosAsync()
  {
    return Task.FromResult(DbEmMemoria.Dados<TEntity>().Values.AsEnumerable());
  }
}

public class ClienteRepository : BaseRepository<Cliente>
{
}

public class VendaRepository : BaseRepository<Venda>
{
}

public class FilialRepository : BaseRepository<Filial>
{
}

public class ProdutoRepository : BaseRepository<Produto>
{
}

public class VendaProdutoRepository : BaseRepository<VendaProduto>
{
}
