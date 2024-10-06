namespace Domain.Entidades;

public record Venda : ILocalEntity
{
  public required Guid Id { get; init; }
  public required Guid IdNumero { get; init; }
  public required DateTime Data { get; init; }
  public required Guid IdCliente { get; init; }
  public required Guid IdFilial { get; init; }
  public bool Fechada { get; set; }
  public bool Cancelado { get; init; }
  public string MotivoCancelamento { get; init; } = "";

  //Desnormalizado de Cliente.Nome
  public required string NomeCliente { get; init; }
  //Desnormalizado de Filial.Nome
  public required string NomeFilial { get; init; }
  //Desnormalizado de Numero
  public required string Numero { get; init; }
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
    IList<VendaProdutoDto> vendaProdutoDtos)
  {
    var valorTotalDaVenda = vendaProdutoDtos
    .Where(vendaProduto => vendaProduto.Cancelado == false)
    .Sum(vendaProtudoDto => vendaProtudoDto.ValorTotalVendaProduto);

    return new VendaCompletaDto
    {
      Id = venda.Id,
      Numero = venda.Numero,
      Data = venda.Data,
      Cliente = new Cliente()
      {
        Id = venda.IdCliente,
        Nome = venda.NomeCliente
      },
      Filial = new Filial()
      {
        Id = venda.IdFilial,
        Nome = venda.NomeFilial
      },
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