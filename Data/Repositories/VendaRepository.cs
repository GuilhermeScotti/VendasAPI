using DataAbstraction.Repositories;
using Domain.Entidades;

namespace Data.Repositories;

public class VendaRepository : BaseRepository<Venda>, IVendaRepository
{
  private readonly IVendaProdutoRepository vendaProdutoRepository;
  private readonly INumeroVendaRepository numeroVendaRepository;

  public VendaRepository(IVendaProdutoRepository vendaProdutoRepository,
  INumeroVendaRepository numeroVendaRepository)
  {
    this.vendaProdutoRepository = vendaProdutoRepository;
    this.numeroVendaRepository = numeroVendaRepository;
  }

  public async Task<Venda> AdicionarDeDtoAsync(CriarVendaDto criarVendaDto)
  {
    using var transação = DbEmMemoria.IniciarTransação();

    try
    {
      var numeroVenda = await numeroVendaRepository.GerarNumeroVendaAsync();

      if (numeroVenda is null)
        throw new Exception("Não foi possívle criar numero de venda.");

      var venda = new Venda
      {
        Id = Guid.NewGuid(),
        IdNumero = numeroVenda.Id,
        Data = DateTime.Now,
        IdCliente = criarVendaDto.IdCliente,
        IdFilial = criarVendaDto.IdFilial,
        Cancelado = false
      };

      DbEmMemoria.Dados<Venda>()[venda.Id] = venda;
      transação.Completar();

      return venda;
    }
    catch (Exception)
    {
      transação.Cancelar();
    }

    throw new Exception("Não foi possívle criar venda.");
  }

  public override async Task<bool> DeletarAsync(Guid id)
  {
    var vendaProdutos = await vendaProdutoRepository.ObterPorIdVendaAsync(id);

    using var transação = DbEmMemoria.IniciarTransação();

    try
    {
      await DeletarVenda(id, vendaProdutos);
      transação.Completar();
    }
    catch (Exception)
    {
      transação.Cancelar();
      //log
    }

    return true;
  }

  public async Task<VendaCompletaDto?> ObterCompletaPorIdAsync(Guid id)
  {
    var venda = await base.ObterPorIdAsync(id);

    if (venda is null) return null;

    var vendaProdutoDtos = await vendaProdutoRepository.ObterDtoPorIdVendaAsync(id);

    var cliente = DbEmMemoria.Dados<Cliente>().Values
    .FirstOrDefault(cliente => cliente.Id == venda.IdCliente)
    ?? throw new InvalidOperationException("Cliente deve existir uma venda");

    var filial = DbEmMemoria.Dados<Filial>().Values
    .FirstOrDefault(filial => filial.Id == venda.IdFilial)
    ?? throw new InvalidOperationException("Filial deve existir uma venda");

    var numeroVenda = DbEmMemoria.Dados<NumeroVenda>().Values
    .FirstOrDefault(numero => numero.Id == venda.IdNumero)
    ?? throw new InvalidOperationException("Filial deve existir uma venda");

    return VendaCompletaDto.ObterDeVenda(venda, vendaProdutoDtos.ToList(), cliente, filial, numeroVenda);
  }

  private Task DeletarVenda(Guid idVenda, IEnumerable<VendaProduto> vendaProdutos)
  {
    if (!DbEmMemoria.Dados<Venda>().TryRemove(idVenda, out _))
    {
      throw new Exception("Falha ao deletar venda");
    }

    foreach (var vendaProduto in vendaProdutos)
    {
      if (!DbEmMemoria.Dados<VendaProduto>().TryRemove(vendaProduto.Id, out _))
      {
        throw new Exception("Falha ao deletar venda produto");
      }
    }

    return Task.CompletedTask;
  }
}
