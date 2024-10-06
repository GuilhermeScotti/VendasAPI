namespace Domain.Entidades;

public record Cliente : IExternalEntity
{
  public required Guid Id { get; set; }
  public required string Nome { get; init; }
}
