namespace Domain.Entidades;

public record Produto : IEntity
{
  public Guid Id { get; init; }
  public required string Nome { get; init; }
  public double ValorUnitario { get; init; }
}
