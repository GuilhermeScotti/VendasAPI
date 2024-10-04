namespace Domain.Entidades;

public record Venda : IEntity
{
  public Guid Id { get; init; }
  public Guid IdNumero { get; init; }
  public DateTime Data { get; init; }
  public Guid IdCliente { get; init; }
  public Guid IdFilial { get; init; }
  public bool Cancelado { get; init; }
  public string MotivoCancelamento { get; init; } = "";
}

public record VendaCompletaDto
{
  public Guid Id { get; init; }
  public required string Numero { get; init; }
  public DateTime Data { get; init; }
  public required Cliente Cliente { get; init; }
  public required Filial Filial { get; init; }
  public bool Cancelado { get; init; }
  public IList<VendaProdutoDto> Produtos { get; init; } = new List<VendaProdutoDto>();
  public double ValorTotalDaVenda { get; init; }

  public static VendaCompletaDto ObterDeVenda(
    Venda venda,
    IList<VendaProdutoDto> vendaProdutoDtos,
    Cliente cliente,
    Filial filial,
    NumeroVenda numeroVenda)
  {
    var valorTotalDaVenda = vendaProdutoDtos
    .Where(vendaProduto => vendaProduto.Cancelado == false)
    .Sum(vendaProtudoDto => vendaProtudoDto.ValorTotalVendaProduto);

    return new VendaCompletaDto
    {
      Id = venda.Id,
      Numero = $"{numeroVenda.Mes}-{numeroVenda.Ano}-{numeroVenda.Numero}",
      Data = venda.Data,
      Cliente = cliente,
      Filial = filial,
      Cancelado = venda.Cancelado,
      Produtos = vendaProdutoDtos,
      ValorTotalDaVenda = valorTotalDaVenda
    };
  }
}

public record CriarVendaDto
{
  public Guid IdCliente { get; init; }
  public Guid IdFilial { get; init; }
}

public record CancelarVendaDto
{
  public required string MotivoCancelamento { get; init; }
}