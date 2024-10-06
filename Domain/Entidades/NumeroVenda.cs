
namespace Domain.Entidades;

public record NumeroVenda : ILocalEntity
{
  public required Guid Id { get; init; }
  public required string Ano { get; init; }
  public required string Mes { get; init; }
  public required int Numero { get; init; }
}