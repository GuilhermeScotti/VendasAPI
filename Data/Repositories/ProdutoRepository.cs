using Domain.Entidades;

namespace Data.Repositories;

public class ProdutoRepository : BaseExternalReadOnlyRepository<Produto>
{
  public ProdutoRepository(IExternalDataContext<Produto> externalDataContext)
: base(externalDataContext)
  {
  }
}
