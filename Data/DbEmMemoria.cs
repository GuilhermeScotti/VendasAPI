using System.Collections.Concurrent;
using DataAbstraction;
using Domain.Entidades;

namespace Data;

internal class DbEmMemoria
{
  static DbEmMemoria()
  {
    AdicionarDadosDeTeste();
  }

  /// <summary>
  /// Apeans para simular uma transação
  /// </summary>
  private class TransaçãoFake : ITransação
  {
    public void Dispose()
    {
    }

    public void Completar() { }
    public void Cancelar() { }
  }

  public static ITransação IniciarTransação()
  {
    return new TransaçãoFake();
  }

  public static ConcurrentDictionary<Guid, Venda> Vendas { get; } = new();
  public static ConcurrentDictionary<Guid, Cliente> Clientes { get; } = new();
  public static ConcurrentDictionary<Guid, Filial> Filiais { get; } = new();
  public static ConcurrentDictionary<Guid, Produto> Produtos { get; } = new();
  public static ConcurrentDictionary<Guid, VendaProduto> VendaProdutos { get; } = new();

  public static ConcurrentDictionary<Guid, T> Dados<T>()
  where T : IEntity
  {
    return typeof(T) switch
    {
      var type when type == typeof(Venda) => (ConcurrentDictionary<Guid, T>)(object)Vendas,
      var type when type == typeof(Cliente) => (ConcurrentDictionary<Guid, T>)(object)Clientes,
      var type when type == typeof(Filial) => (ConcurrentDictionary<Guid, T>)(object)Filiais,
      var type when type == typeof(Produto) => (ConcurrentDictionary<Guid, T>)(object)Produtos,
      var type when type == typeof(VendaProduto) => (ConcurrentDictionary<Guid, T>)(object)VendaProdutos,
      _ => throw new ArgumentException("Tipo desconhecido")
    };
  }

  private static void AdicionarDadosDeTeste()
  {
    var cliente1 = new Cliente { Id = Guid.NewGuid(), Nome = "José Maria" };
    var cliente2 = new Cliente { Id = Guid.NewGuid(), Nome = "João Pedro" };

    var filial1 = new Filial { Id = Guid.NewGuid(), Nome = "Filial Centro" };
    var filial2 = new Filial { Id = Guid.NewGuid(), Nome = "Filial Zona Sul" };

    var produto1 = new Produto { Id = Guid.NewGuid(), ValorUnitario = 10.0, Nome = "Cadeira" };
    var produto2 = new Produto { Id = Guid.NewGuid(), ValorUnitario = 20.0, Nome = "Mesa" };

    Clientes[cliente1.Id] = cliente1;
    Clientes[cliente2.Id] = cliente2;

    Filiais[filial1.Id] = filial1;
    Filiais[filial2.Id] = filial2;

    Produtos[produto1.Id] = produto1;
    Produtos[produto2.Id] = produto2;

    var venda1 = new Venda
    {
      Id = Guid.NewGuid(),
      Numero = "V001",
      Data = DateTime.UtcNow,
      IdCliente = cliente1.Id,
      IdFilial = filial1.Id
    };
    Vendas[venda1.Id] = venda1;

    var vendaProduto1 = new VendaProduto
    {
      Id = Guid.NewGuid(),
      IdVenda = venda1.Id,
      IdProduto = produto1.Id,
      Quantidade = 2,
      PorcentagemDesconto = 0,
      Cancelado = false
    };
    VendaProdutos[vendaProduto1.Id] = vendaProduto1;

    var venda2 = new Venda
    {
      Id = Guid.NewGuid(),
      Numero = "V002",
      Data = DateTime.UtcNow.AddDays(-1),
      IdCliente = cliente2.Id,
      IdFilial = filial2.Id
    };
    Vendas[venda2.Id] = venda2;

    var vendaProduto2 = new VendaProduto
    {
      Id = Guid.NewGuid(),
      IdVenda = venda2.Id,
      IdProduto = produto2.Id,
      Quantidade = 1,
      PorcentagemDesconto = 7.5,
      Cancelado = false
    };
    VendaProdutos[vendaProduto2.Id] = vendaProduto2;
  }
}
