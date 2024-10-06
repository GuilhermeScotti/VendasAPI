using FluentAssertions;
using NSubstitute;
using Data.Repositories;
using Domain.Entidades;
using Bogus;
using DataAbstraction;
namespace Testes;

//Testes para VendaProdutoRepository como demonstração.
//As demais classes seriam testadas de forma semelhante.
//Bem como mais testes para VendaProdutoRepository.

public class VendaProdutoRepositoryTests
{
  private readonly ILocalDataContext<VendaProduto> _localDataContext;
  private readonly IExternalDataContext<Produto> _produtoExternalContext;
  private readonly VendaProdutoRepository _vendaProdutoRepository;

  public VendaProdutoRepositoryTests()
  {
    _localDataContext = Substitute.For<ILocalDataContext<VendaProduto>>();
    _produtoExternalContext = Substitute.For<IExternalDataContext<Produto>>();
    _vendaProdutoRepository = new VendaProdutoRepository(_localDataContext, _produtoExternalContext);
  }

  [Fact]
  public async Task VenderProdutosAsync_DeveAdicionarProdutosCorretamente()
  {
    // Arrange
    var idVenda = Guid.NewGuid();

    var faker = new Faker();
    var produtoFake = new Produto
    {
      Id = faker.Random.Guid(),
      Nome = "Produto de teste",
      ValorUnitario = faker.Random.Double(10, 100)
    };

    var venderProdutosDto = new VenderProdutosDto
    {
      VenderProdutos =
            [
                new VenderProdutoDto
                {
                  IdProduto = produtoFake.Id,
                  Quantidade = faker.Random.Int(1, 10),
                  PorcentagemDesconto = faker.Random.Double(1, 5)
                }
            ]
    };

    _produtoExternalContext.Dados().Returns(new Dictionary<Guid, Produto>
        {
            { produtoFake.Id, produtoFake }
        });

    var vendaProdutoDictionary = new Dictionary<Guid, VendaProduto>();
    _localDataContext.Dados().Returns(vendaProdutoDictionary);
    _localDataContext.IniciarTransação().Returns(Substitute.For<ITransação>());

    // Act
    await _vendaProdutoRepository.VenderProdutosAsync(idVenda, venderProdutosDto);

    // Assert
    vendaProdutoDictionary.Should().ContainKey(vendaProdutoDictionary.Values.First().Id);
  }

  [Fact]
  public async Task VenderProdutosAsync_DeveLancarExcecao_SeProdutoNaoExistir()
  {
    // Arrange
    var idVenda = Guid.NewGuid();
    var venderProdutosDto = new VenderProdutosDto
    {
      VenderProdutos =
      [
        new VenderProdutoDto
        {
          IdProduto = Guid.NewGuid(),
          Quantidade = 2,
          PorcentagemDesconto = 0
        }
      ]
    };

    _produtoExternalContext.Dados().Returns(new Dictionary<Guid, Produto>());

    // Act
    Func<Task> act = async () => await _vendaProdutoRepository.VenderProdutosAsync(idVenda, venderProdutosDto);

    // Assert
    await act.Should().ThrowAsync<Exception>()
        .WithMessage($"Produto {venderProdutosDto.VenderProdutos[0].IdProduto} não existe");
  }
}