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
  public static ConcurrentDictionary<Guid, NumeroVenda> NumeroVendas { get; } = new();
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
      var type when type == typeof(NumeroVenda) => (ConcurrentDictionary<Guid, T>)(object)NumeroVendas,
      _ => throw new ArgumentException("Tipo desconhecido")
    };
  }

  private static void AdicionarDadosDeTeste()
  {
    var cliente2 = new Cliente { Id = Guid.NewGuid(), Nome = "João Pedro" };

    //Cliente usado pra criar uma venda no postman
    var cliente1 = new Cliente { Id = Guid.Parse("9eb0bd40-0169-4096-8fd5-95e6dadf6a94"), Nome = "José Maria" };

    var filial1 = new Filial { Id = Guid.NewGuid(), Nome = "Filial Centro" };

    //Filial usada pra criar uma venda no postman
    var filial2 = new Filial { Id = Guid.Parse("49bc7d4f-630f-4532-97c7-67543addf605"), Nome = "Filial Zona Sul" };

    var produto1 = new Produto { Id = Guid.NewGuid(), ValorUnitario = 10.0, Nome = "Cadeira" };
    var produto2 = new Produto { Id = Guid.NewGuid(), ValorUnitario = 20.0, Nome = "Mesa" };
    var produto3 = new Produto { Id = Guid.NewGuid(), ValorUnitario = 20.0, Nome = "Armário" };
    var produto4 = new Produto { Id = Guid.NewGuid(), ValorUnitario = 20.0, Nome = "Vaso" };

    var numeroVenda1 = new NumeroVenda { Id = Guid.NewGuid(), Ano = "2024", Mes = "04", Numero = 2 };
    var numeroVenda2 = new NumeroVenda { Id = Guid.NewGuid(), Ano = "2024", Mes = "05", Numero = 1 };

    Clientes[cliente1.Id] = cliente1;
    Clientes[cliente2.Id] = cliente2;

    Filiais[filial1.Id] = filial1;
    Filiais[filial2.Id] = filial2;

    Produtos[produto1.Id] = produto1;
    Produtos[produto2.Id] = produto2;
    Produtos[produto3.Id] = produto3;
    Produtos[produto4.Id] = produto4;

    NumeroVendas[numeroVenda1.Id] = numeroVenda1;
    NumeroVendas[numeroVenda2.Id] = numeroVenda2;

    var venda1 = new Venda
    {
      //Venda deletada no postman
      Id = Guid.Parse("e9ec29f8-d539-4c9f-9c0b-5a7d585860dc"),
      IdNumero = numeroVenda1.Id,
      Data = DateTime.UtcNow,
      IdCliente = cliente1.Id,
      IdFilial = filial1.Id,
      Cancelado = false
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
      //Venda no GET de venda completa no postman
      Id = Guid.Parse("5b165226-bf9a-4960-8a88-a99f9fe16f5a"),
      IdNumero = numeroVenda2.Id,
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

    var vendaProduto3 = new VendaProduto
    {
      Id = Guid.NewGuid(),
      IdVenda = venda2.Id,
      IdProduto = produto3.Id,
      Quantidade = 3,
      PorcentagemDesconto = 0,
      Cancelado = false
    };
    VendaProdutos[vendaProduto3.Id] = vendaProduto3;

    var vendaProduto4 = new VendaProduto
    {
      Id = Guid.NewGuid(),
      IdVenda = venda2.Id,
      IdProduto = produto4.Id,
      Quantidade = 7,
      PorcentagemDesconto = 3.5,
      Cancelado = false
    };
    VendaProdutos[vendaProduto4.Id] = vendaProduto4;
  }
}
