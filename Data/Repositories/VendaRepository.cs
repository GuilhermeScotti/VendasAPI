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

  public override async Task<bool> DeletarAsync(Guid id)
  {
    var vendaProdutos = await vendaProdutoRepository.ObterPorIdVendaAsync(id);

    using var transação = DbEmMemoria.IniciarTransação();

    try
    {
      await DeletarVenda(id, vendaProdutos);
      transação.Completar();
    }
    catch (Exception)
    {
      transação.Cancelar();
      //log
    }

    return true;
  }

  public async Task<VendaCompletaDto?> ObterCompletaPorIdAsync(Guid id)
  {
    var venda = await base.ObterPorIdAsync(id);

    if (venda is null) return null;

    var vendaProdutoDtos = await vendaProdutoRepository.ObterDtoPorIdVendaAsync(id);

    var cliente = DbEmMemoria.Dados<Cliente>().Values
    .FirstOrDefault(cliente => cliente.Id == venda.IdCliente)
    ?? throw new InvalidOperationException("Cliente deve existir uma venda");

    var filial = DbEmMemoria.Dados<Filial>().Values
    .FirstOrDefault(filial => filial.Id == venda.IdFilial)
    ?? throw new InvalidOperationException("Filial deve existir uma venda");

    return VendaCompletaDto.ObterDeVenda(venda, vendaProdutoDtos.ToList(), cliente, filial);
  }

  private Task DeletarVenda(Guid idVenda, IEnumerable<VendaProduto> vendaProdutos)
  {
    if (!DbEmMemoria.Dados<Venda>().TryRemove(idVenda, out _))
    {
      throw new Exception("Falha ao deletar venda");
    }

    foreach (var vendaProduto in vendaProdutos)
    {
      if (!DbEmMemoria.Dados<VendaProduto>().TryRemove(vendaProduto.Id, out _))
      {
        throw new Exception("Falha ao deletar venda produto");
      }
    }

    return Task.CompletedTask;
  }
}
