using DataAbstraction.Repositories;
using Domain.Entidades;

namespace Data.Repositories;

public class NumeroVendaRepository : BaseRepository<NumeroVenda>, INumeroVendaRepository
{
  private const int MAX_TENTATIVAS = 5;

  public Task<NumeroVenda?> GerarNumeroVendaAsync()
  {
    var tentativa = 1;

    do
    {
      using var transação = DbEmMemoria.IniciarTransação();

      try
      {
        var mes = DateTime.Now.Month.ToString("D2");
        var ano = DateTime.Now.Year.ToString();
        var numeroGerado = GerarNumero(mes, ano);

        var numeroVenda = new NumeroVenda
        {
          Id = Guid.NewGuid(),
          Ano = ano,
          Mes = mes,
          Numero = numeroGerado
        };

        DbEmMemoria.Dados<NumeroVenda>()[numeroVenda.Id] = numeroVenda;

        transação.Completar();
        return Task.FromResult<NumeroVenda?>(numeroVenda);
      }
      catch (Exception)
      {
        transação.Cancelar();
        tentativa++;
      }

    } while (tentativa < MAX_TENTATIVAS);

    return Task.FromResult<NumeroVenda?>(null);
  }

  private int GerarNumero(string mes, string ano)
  {
    var ultimoNumeroVenda = DbEmMemoria.Dados<NumeroVenda>().Values
    .Where(numero => numero.Mes == mes && numero.Ano == ano)
    .OrderByDescending(venda => venda.Numero)
    .Select(venda => venda.Numero)
    .FirstOrDefault();

    return ultimoNumeroVenda + 1;
  }
}