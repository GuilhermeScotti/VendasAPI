namespace Domain.Entidades;

public record Filial : IEntity
{
  public Guid Id { get; init; }
  public required string Nome { get; init; }
}