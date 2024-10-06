namespace Domain.Entidades;

public record VendaProduto : ILocalEntity
{
  public required Guid Id { get; init; }
  public required Guid IdVenda { get; init; }
  public required Guid IdProduto { get; init; }
  public required int Quantidade { get; init; }
  public required double PorcentagemDesconto { get; init; }
  public bool Cancelado { get; init; }

  //Desnormalizado de Produto.ValorUnitário
  public required double ValorUnitario { get; init; }
}

public record VendaProdutoDto
{
  public required Guid Id { get; init; }
  public required Guid IdVenda { get; init; }
  public required Guid IdProduto { get; init; }
  public required int Quantidade { get; init; }
  public required double PorcentagemDesconto { get; init; }
  public required bool Cancelado { get; init; }
  public required double ValorUnitarioProduto { get; set; }
  public required double ValorTotalVendaProduto { get; set; }

  public static VendaProdutoDto ObterDeVendaProduto(
    VendaProduto vendaProduto)
  {
    double valorTotalVendaProduto =
    vendaProduto.ValorUnitario * vendaProduto.Quantidade * (1 - (vendaProduto.PorcentagemDesconto / 100));

    return new VendaProdutoDto
    {
      Id = vendaProduto.Id,
      IdVenda = vendaProduto.IdVenda,
      IdProduto = vendaProduto.IdProduto,
      Quantidade = vendaProduto.Quantidade,
      PorcentagemDesconto = vendaProduto.PorcentagemDesconto,
      Cancelado = vendaProduto.Cancelado,
      ValorUnitarioProduto = vendaProduto.ValorUnitario,
      ValorTotalVendaProduto = valorTotalVendaProduto
    };
  }
}

public record VenderProdutosDto
{
  public required VenderProdutoDto[] VenderProdutos { get; init; }
}

public record VenderProdutoDto
{
  public Guid IdProduto { get; init; }
  public int Quantidade { get; init; }
  public double PorcentagemDesconto { get; init; }
}
