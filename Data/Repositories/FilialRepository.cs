using Domain.Entidades;

namespace Data.Repositories;

public class FilialRepository : BaseExternalReadOnlyRepository<Filial>
{
  public FilialRepository(IExternalDataContext<Filial> externalDataContext)
: base(externalDataContext)
  {
  }
}
