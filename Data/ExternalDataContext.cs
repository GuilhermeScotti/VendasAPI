using Domain.Entidades;

namespace Data;

public class ExternalDataContext<TExternalEntity> : IExternalDataContext<TExternalEntity>
where TExternalEntity : IExternalEntity
{
  public IReadOnlyDictionary<Guid, TExternalEntity> Dados()
  {
    return DadosParaTeste.Dados<TExternalEntity>();
  }
}


