public class ServiçoDeMensageria : IServiçoDeMensageria
{
  public Task VendaCriada(Guid IdVenda)
  {
    Console.WriteLine($"*** Venda criada: {IdVenda} ***");
    return Task.CompletedTask;
  }

  public Task VendaAlterada(Guid IdVenda)
  {
    Console.WriteLine($"*** Venda alterada: {IdVenda} ***");
    return Task.CompletedTask;
  }

  public Task VendaCancelada(Guid IdVenda)
  {
    Console.WriteLine($"*** Venda cancelada: {IdVenda} ***");
    return Task.CompletedTask;
  }

  public Task VendaFechada(Guid IdVenda)
  {
    Console.WriteLine($"*** Venda fechada: {IdVenda} ***");
    return Task.CompletedTask;
  }

  public Task ItemCancelado(Guid IdProduto)
  {
    Console.WriteLine($"*** Item cancelado: {IdProduto} ***");
    return Task.CompletedTask;
  }

  public Task VendaExcluida(Guid IdVenda)
  {
    Console.WriteLine($"*** Venda excluída: {IdVenda} ***");
    return Task.CompletedTask;
  }
}

public interface IServiçoDeMensageria
{
  Task VendaCriada(Guid IdVenda);
  Task VendaAlterada(Guid IdVenda);
  Task VendaCancelada(Guid IdVenda);
  Task VendaFechada(Guid IdVenda);
  Task ItemCancelado(Guid IdProduto);
  Task VendaExcluida(Guid IdVenda);
}