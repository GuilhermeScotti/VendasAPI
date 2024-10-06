using DataAbstraction.Repositories;
using Domain.Entidades;

namespace Data.Repositories;

public class VendaRepository : BaseRepository<Venda>, IVendaRepository
{
  private readonly IVendaProdutoRepository vendaProdutoRepository;
  private readonly INumeroVendaRepository numeroVendaRepository;
  private readonly ILocalDataContext<Venda> vendaContext;
  private readonly IExternalDataContext<Filial> filialExternalContext;
  private readonly IExternalDataContext<Cliente> clienteExternalContext;
  private readonly ILocalDataContext<VendaProduto> vendaProdutoContext;

  public VendaRepository(IVendaProdutoRepository vendaProdutoRepository,
  INumeroVendaRepository numeroVendaRepository, ILocalDataContext<Venda> vendaContext,
  IExternalDataContext<Filial> filialExternalContext, IExternalDataContext<Cliente> clienteExternalContext,
  ILocalDataContext<VendaProduto> vendaProdutoContext) : base(vendaContext)
  {
    this.vendaProdutoRepository = vendaProdutoRepository;
    this.numeroVendaRepository = numeroVendaRepository;
    this.vendaContext = vendaContext;
    this.filialExternalContext = filialExternalContext;
    this.clienteExternalContext = clienteExternalContext;
    this.vendaProdutoContext = vendaProdutoContext;
  }

  public async Task<Venda> AdicionarDeDtoAsync(CriarVendaDto criarVendaDto)
  {
    using var transação = vendaContext.IniciarTransação();

    try
    {
      var cliente = clienteExternalContext.Dados()
      .Values
      .FirstOrDefault(cliente => cliente.Id == criarVendaDto.IdCliente)
      ?? throw new Exception("Cliente inexistente.");

      var filial = filialExternalContext.Dados()
      .Values
      .FirstOrDefault(filial => filial.Id == criarVendaDto.IdFilial)
      ?? throw new Exception("Filial inexistente."); ;

      var numeroVenda = await numeroVendaRepository.GerarNumeroVendaAsync();

      if (numeroVenda is null)
        throw new Exception("Não foi possívle criar numero de venda.");

      var venda = new Venda
      {
        Id = Guid.NewGuid(),
        IdNumero = numeroVenda.Id,
        Data = DateTime.Now,
        IdCliente = criarVendaDto.IdCliente,
        NomeCliente = cliente.Nome,
        IdFilial = criarVendaDto.IdFilial,
        NomeFilial = filial.Nome,
        Numero = $"{numeroVenda.Mes}-{numeroVenda.Ano}-{numeroVenda.Numero}"
      };

      vendaContext.Dados()[venda.Id] = venda;
      transação.Completar();

      return venda;
    }
    catch (Exception)
    {
      transação.Cancelar();
      throw;
    }
  }

  public override async Task<bool> DeletarAsync(Guid id)
  {
    var vendaProdutos = await vendaProdutoRepository.ObterPorIdVendaAsync(id);

    using var transação = vendaContext.IniciarTransação();

    try
    {
      await DeletarVenda(id, vendaProdutos);
      transação.Completar();
    }
    catch (Exception)
    {
      transação.Cancelar();
      throw;
    }

    return true;
  }

  public async Task<VendaCompletaDto?> ObterCompletaPorIdAsync(Guid id)
  {
    var venda = await base.ObterPorIdAsync(id);

    if (venda is null) return null;

    var vendaProdutoDtos = await vendaProdutoRepository.ObterDtoPorIdVendaAsync(id);

    return VendaCompletaDto.ObterDeVenda(venda, vendaProdutoDtos.ToList());
  }

  private Task DeletarVenda(Guid idVenda, IEnumerable<VendaProduto> vendaProdutos)
  {
    if (!vendaContext.Dados().Remove(idVenda, out _))
    {
      throw new Exception("Falha ao deletar venda");
    }

    foreach (var vendaProduto in vendaProdutos)
    {
      if (!vendaProdutoContext.Dados().Remove(vendaProduto.Id, out _))
      {
        throw new Exception("Falha ao deletar venda produto");
      }
    }

    return Task.CompletedTask;
  }
}
