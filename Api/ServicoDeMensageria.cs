public class ServiçoDeMensageria : IServiçoDeMensageria
{
  public Task CompraCriada()
  {
    Console.WriteLine("*** Compra criada ***");
    return Task.CompletedTask;
  }

  public Task CompraAlterada()
  {
    Console.WriteLine("*** Compra alterada ***");
    return Task.CompletedTask;
  }

  public Task CompraCancelada()
  {
    Console.WriteLine("*** Compra cancelada ***");
    return Task.CompletedTask;
  }

  public Task ItemCancelado()
  {
    Console.WriteLine("*** Item cancelado ***");
    return Task.CompletedTask;
  }

  public Task CompraExcluida()
  {
    Console.WriteLine("*** Item cancelado ***");
    return Task.CompletedTask;
  }
}

public interface IServiçoDeMensageria
{
  Task CompraCriada();
  Task CompraAlterada();
  Task CompraCancelada();
  Task ItemCancelado();
  Task CompraExcluida();
}