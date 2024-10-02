using Domain.Entidades;

namespace DataAbstraction.Repositories;

public interface IRepository<TEntity>
where TEntity : IEntity
{
  Task<IEnumerable<TEntity>> ObterTodosAsync();
  Task<TEntity?> ObterPorIdAsync(Guid id);
  Task AdicionarAsync(TEntity entidade);
  Task AtualizarAsync(TEntity entidade);
  Task<bool> DeletarAsync(Guid id);
}

public interface IVendaProdutoRepository : IRepository<VendaProduto>
{
  Task<IEnumerable<VendaProduto>> ObterPorIdVendaAsync(Guid idVenda);
}

public interface IVendaRepository : IRepository<Venda>
{
  Task<VendaCompletaDto?> ObterCompletaPorIdAsync(Guid id);
}