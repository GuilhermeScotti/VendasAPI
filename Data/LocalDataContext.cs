using DataAbstraction;
using Domain.Entidades;

namespace Data;

public class LocalDataContext<TLocalEntity> : ILocalDataContext<TLocalEntity>
  where TLocalEntity : ILocalEntity
{
  public IDictionary<Guid, TLocalEntity> Dados()
  {
    return DadosParaTeste.Dados<TLocalEntity>();
  }

  public ITransação IniciarTransação()
  {
    return DadosParaTeste.IniciarTransação();
  }

}


