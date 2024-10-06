namespace Domain.Entidades;

public record Produto : IExternalEntity
{
  public required Guid Id { get; init; }
  public required string Nome { get; init; }
  public required double ValorUnitario { get; init; }
}
