using DataAbstraction.Repositories;
using Domain.Entidades;

namespace Data.Repositories;

public class VendaRepository : BaseRepository<Venda>
{
  private readonly IVendaProdutoRepository vendaProdutoRepository;

  public VendaRepository(IVendaProdutoRepository vendaProdutoRepository)
  {
    this.vendaProdutoRepository = vendaProdutoRepository;
  }

  public async override Task<Venda?> ObterPorIdAsync(Guid id)
  {
    var venda = await base.ObterPorIdAsync(id);

    if (venda is null) return null;

    var vendaProdutos = await vendaProdutoRepository.ObterPorIdVendaAsync(id);

    return venda with { Produtos = vendaProdutos.ToList() };
  }
}
