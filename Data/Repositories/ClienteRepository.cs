using Domain.Entidades;

namespace Data.Repositories;

public class ClienteRepository : BaseExternalReadOnlyRepository<Cliente>
{
  public ClienteRepository(IExternalDataContext<Cliente> externalDataContext)
  : base(externalDataContext)
  {
  }
}
