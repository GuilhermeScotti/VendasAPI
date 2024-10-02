namespace DataAbstraction;

public interface ITransação : IDisposable
{
  void Completar();
  void Cancelar();
}