using Domain.Entidades;

namespace DataAbstraction.Repositories;

public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>
where TEntity : IEntity
{
  Task AdicionarAsync(TEntity entidade);
  Task AtualizarAsync(TEntity entidade);
  Task<bool> DeletarAsync(Guid id);
}

public interface IReadOnlyRepository<TEntity>
where TEntity : IEntity
{
  Task<IEnumerable<TEntity>> ObterTodosAsync();
  Task<TEntity?> ObterPorIdAsync(Guid id);
}

public interface IVendaProdutoRepository : IRepository<VendaProduto>
{
  Task<IEnumerable<VendaProduto>> ObterPorIdVendaAsync(Guid idVenda);
  Task<IEnumerable<VendaProdutoDto>> ObterDtoPorIdVendaAsync(Guid idVenda);

  Task VenderProdutosAsync(Guid idVenda, VenderProdutosDto venderProdutosDto);
}

public interface IVendaRepository : IRepository<Venda>
{
  Task<VendaCompletaDto?> ObterCompletaPorIdAsync(Guid id);
  Task<Venda> AdicionarDeDtoAsync(CriarVendaDto criarVendaDto);
}

public interface INumeroVendaRepository : IRepository<NumeroVenda>
{
  Task<NumeroVenda?> GerarNumeroVendaAsync();
}
