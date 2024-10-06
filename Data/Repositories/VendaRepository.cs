using DataAbstraction.Repositories;
using Domain.Entidades;

namespace Data.Repositories;

public class VendaRepository : BaseRepository<Venda>, IVendaRepository
{
  private readonly IVendaProdutoRepository _vendaProdutoRepository;
  private readonly INumeroVendaRepository _numeroVendaRepository;
  private readonly ILocalDataContext<Venda> _vendaContext;
  private readonly IExternalDataContext<Filial> _filialExternalContext;
  private readonly IExternalDataContext<Cliente> _clienteExternalContext;
  private readonly ILocalDataContext<VendaProduto> _vendaProdutoContext;

  public VendaRepository(IVendaProdutoRepository vendaProdutoRepository,
  INumeroVendaRepository numeroVendaRepository, ILocalDataContext<Venda> vendaContext,
  IExternalDataContext<Filial> filialExternalContext, IExternalDataContext<Cliente> clienteExternalContext,
  ILocalDataContext<VendaProduto> vendaProdutoContext) : base(vendaContext)
  {
    _vendaProdutoRepository = vendaProdutoRepository;
    _numeroVendaRepository = numeroVendaRepository;
    _vendaContext = vendaContext;
    _filialExternalContext = filialExternalContext;
    _clienteExternalContext = clienteExternalContext;
    _vendaProdutoContext = vendaProdutoContext;
  }

  public async Task<Venda> AdicionarDeDtoAsync(CriarVendaDto criarVendaDto)
  {
    using var transação = _vendaContext.IniciarTransação();

    try
    {
      var cliente = _clienteExternalContext.Dados()
      .Values
      .FirstOrDefault(cliente => cliente.Id == criarVendaDto.IdCliente)
      ?? throw new Exception("Cliente inexistente.");

      var filial = _filialExternalContext.Dados()
      .Values
      .FirstOrDefault(filial => filial.Id == criarVendaDto.IdFilial)
      ?? throw new Exception("Filial inexistente."); ;

      var numeroVenda = await _numeroVendaRepository.GerarNumeroVendaAsync();

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

      _vendaContext.Dados()[venda.Id] = venda;
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
    var vendaProdutos = await _vendaProdutoRepository.ObterPorIdVendaAsync(id);

    using var transação = _vendaContext.IniciarTransação();

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

    var vendaProdutoDtos = await _vendaProdutoRepository.ObterDtoPorIdVendaAsync(id);

    return VendaCompletaDto.ObterDeVenda(venda, vendaProdutoDtos.ToList());
  }

  private Task DeletarVenda(Guid idVenda, IEnumerable<VendaProduto> vendaProdutos)
  {
    if (!_vendaContext.Dados().Remove(idVenda, out _))
    {
      throw new Exception("Falha ao deletar venda");
    }

    foreach (var vendaProduto in vendaProdutos)
    {
      if (!_vendaProdutoContext.Dados().Remove(vendaProduto.Id, out _))
      {
        throw new Exception("Falha ao deletar venda produto");
      }
    }

    return Task.CompletedTask;
  }
}
