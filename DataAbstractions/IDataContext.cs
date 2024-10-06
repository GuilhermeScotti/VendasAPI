using DataAbstraction;
using Domain.Entidades;

public interface ILocalDataContext<TLocalEntity>
  where TLocalEntity : ILocalEntity
{
  IDictionary<Guid, TLocalEntity> Dados();
  ITransação IniciarTransação();
}

public interface IExternalDataContext<TExternalEntity>
where TExternalEntity : IExternalEntity
{
  IReadOnlyDictionary<Guid, TExternalEntity> Dados();
}