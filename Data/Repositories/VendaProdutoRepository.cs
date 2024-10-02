using DataAbstraction.Repositories;
using Domain.Entidades;

namespace Data.Repositories;

public class VendaProdutoRepository : BaseRepository<VendaProduto>, IVendaProdutoRepository
{
  public Task<IEnumerable<VendaProduto>> ObterPorIdVendaAsync(Guid idVenda)
  {
    var vendaProdutos = DbEmMemoria.Dados<VendaProduto>()
    .Values
    .Where(vendaProd => vendaProd.IdVenda == idVenda);

    return Task.FromResult(vendaProdutos);
  }
}
