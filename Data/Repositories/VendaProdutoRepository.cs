using DataAbstraction.Repositories;
using Domain.Entidades;

namespace Data.Repositories;

public class VendaProdutoRepository : BaseRepository<VendaProduto>, IVendaProdutoRepository
{
  public Task<IEnumerable<VendaProduto>> ObterPorIdVendaAsync(Guid idVenda)
  {
    var vendaProdutos = DbEmMemoria.Dados<VendaProduto>()
    .Values
    .Where(vendaProd => vendaProd.IdVenda == idVenda);

    return Task.FromResult(vendaProdutos);
  }

  public Task<IEnumerable<VendaProdutoDto>> ObterDtoPorIdVendaAsync(Guid idVenda)
  {
    var vendaProdutos = DbEmMemoria.Dados<VendaProduto>()
    .Values
    .Where(vendaProd => vendaProd.IdVenda == idVenda);

    var vendaProdutoDtos = vendaProdutos
    .Select(vendaProduto =>
    {
      var produto = DbEmMemoria.Dados<Produto>().Values
      .FirstOrDefault(produto => produto.Id == vendaProduto.IdProduto)
      ?? throw new InvalidOperationException("Produto deve existir em uma venda de produto");

      return VendaProdutoDto.ObterDeVendaProduto(vendaProduto, produto.ValorUnitario);
    });

    return Task.FromResult(vendaProdutoDtos);
  }
}
