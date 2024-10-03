namespace Domain.Entidades;

public record VendaProduto : IEntity
{
  public Guid Id { get; init; }
  public Guid IdVenda { get; init; }
  public Guid IdProduto { get; init; }
  public int Quantidade { get; init; }
  public double PorcentagemDesconto { get; init; }
  public bool Cancelado { get; init; }
}

public record VendaProdutoDto
{
  public Guid Id { get; init; }
  public Guid IdVenda { get; init; }
  public Guid IdProduto { get; init; }
  public int Quantidade { get; init; }
  public double PorcentagemDesconto { get; init; }
  public bool Cancelado { get; init; }
  public double ValorUnitarioProduto { get; set; }
  public double ValorTotalVendaProduto { get; set; }

  public static VendaProdutoDto ObterDeVendaProduto(
    VendaProduto vendaProduto,
    double valorUnitarioProduto)
  {
    double valorTotalVendaProduto =
    valorUnitarioProduto * vendaProduto.Quantidade * (1 - (vendaProduto.PorcentagemDesconto / 100));

    return new VendaProdutoDto
    {
      Id = vendaProduto.Id,
      IdVenda = vendaProduto.IdVenda,
      IdProduto = vendaProduto.IdProduto,
      Quantidade = vendaProduto.Quantidade,
      PorcentagemDesconto = vendaProduto.PorcentagemDesconto,
      Cancelado = vendaProduto.Cancelado,
      ValorUnitarioProduto = valorUnitarioProduto,
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
