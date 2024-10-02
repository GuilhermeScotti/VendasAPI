namespace Domain.Entidades;

public record Cliente : IEntity
{
  public Guid Id { get; set; }
  public required string Nome { get; init; }
}
