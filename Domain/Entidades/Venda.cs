﻿namespace Domain.Entidades;

public record Venda : IEntity
{
  public Guid Id { get; init; }
  public required string Numero { get; init; }
  public DateTime Data { get; init; }
  public Guid IdCliente { get; init; }
  public Guid IdFilial { get; init; }
  public List<VendaProduto> Produtos { get; init; } = new();
  public bool Cancelado { get; init; }
}
