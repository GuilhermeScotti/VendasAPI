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
