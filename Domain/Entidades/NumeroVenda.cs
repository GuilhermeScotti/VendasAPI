
namespace Domain.Entidades;

public record NumeroVenda : ILocalEntity
{
  public Guid Id { get; init; }
  public required string Ano { get; init; }
  public required string Mes { get; init; }
  public int Numero { get; init; }
}