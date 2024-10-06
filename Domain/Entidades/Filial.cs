namespace Domain.Entidades;

public record Filial : IExternalEntity
{
  public required Guid Id { get; init; }
  public required string Nome { get; init; }
}