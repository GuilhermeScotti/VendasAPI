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
