using DataAbstraction.Repositories;
using Domain.Entidades;

namespace Data.Repositories;

public class VendaRepository : BaseRepository<Venda>, IVendaRepository
{
  private readonly IVendaProdutoRepository vendaProdutoRepository;

  public VendaRepository(IVendaProdutoRepository vendaProdutoRepository)
  {
    this.vendaProdutoRepository = vendaProdutoRepository;
  }

  public async Task<VendaCompletaDto?> ObterCompletaPorIdAsync(Guid id)
  {
    var venda = await base.ObterPorIdAsync(id);

    if (venda is null) return null;

    var vendaProdutos = await vendaProdutoRepository.ObterPorIdVendaAsync(id);

    return new VendaCompletaDto
    {
      Id = venda.Id,
      Numero = venda.Numero,
      Data = venda.Data,
      IdCliente = venda.IdCliente,
      IdFilial = venda.IdFilial,
      Cancelado = venda.Cancelado,
      Produtos = vendaProdutos.ToList()
    };
  }
}
