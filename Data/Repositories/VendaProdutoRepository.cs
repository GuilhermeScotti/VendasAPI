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

  public async Task VenderProdutosAsync(Guid idVenda, VenderProdutosDto venderProdutosDto)
  {
    using var transação = DbEmMemoria.IniciarTransação();

    try
    {
      await VenderProdutosInternal(idVenda, venderProdutosDto);
      transação.Completar();
    }
    catch (Exception)
    {
      transação.Cancelar();
      throw;
    }
  }

  private async Task VenderProdutosInternal(Guid idVenda,
  VenderProdutosDto venderProdutosDto)
  {
    foreach (var vendaProdutoDto in venderProdutosDto.VenderProdutos)
    {
      var produtoExiste = DbEmMemoria.Dados<Produto>().Values
      .FirstOrDefault(produto => produto.Id == vendaProdutoDto.IdProduto) is not null;

      if (!produtoExiste)
        throw new Exception($"Produto {vendaProdutoDto.IdProduto} não existe");

      var novaVendaProduto = new VendaProduto
      {
        Id = Guid.NewGuid(),
        IdVenda = idVenda,
        IdProduto = vendaProdutoDto.IdProduto,
        Quantidade = vendaProdutoDto.Quantidade,
        PorcentagemDesconto = vendaProdutoDto.PorcentagemDesconto,
        Cancelado = false
      };

      await base.AdicionarAsync(novaVendaProduto);
    }
  }
}
