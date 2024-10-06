using Domain.Entidades;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using System.Text.Json;

public class DemoTesteIntegracao : IAsyncLifetime
{
  private readonly IContainer _apiContainer;

  public DemoTesteIntegracao()
  {
    _apiContainer = new ContainerBuilder()
      .WithImage("api-de-vendas")
      .WithPortBinding(5063, false)
      .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(5063)))
      .Build();
  }

  public async Task InitializeAsync()
  {
    await _apiContainer.StartAsync();
  }

  public async Task DisposeAsync()
  {
    await _apiContainer.StopAsync();
    await _apiContainer.DisposeAsync();
  }

  [Fact]
  public async Task Teste_container_ah()
  {
    const string guidString = "e9ec29f8-d539-4c9f-9c0b-5a7d585860dc";

    var httpClient = new HttpClient();

    var requestUri = new UriBuilder(Uri.UriSchemeHttp, _apiContainer.Hostname, _apiContainer.GetMappedPublicPort(5063), "vendas").Uri;

    var jsonResponse = await httpClient.GetStringAsync(requestUri);

    var options = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true,
    };

    var vendas = JsonSerializer.Deserialize<List<Venda>>(jsonResponse, options);

    Assert.NotNull(vendas);

    var vendaASerDeletada = vendas.FirstOrDefault(venda => venda.Id == Guid.Parse(guidString));

    Assert.NotNull(vendaASerDeletada);

    var deleteUri = new UriBuilder(Uri.UriSchemeHttp, _apiContainer.Hostname, _apiContainer.GetMappedPublicPort(5063), $"vendas/{guidString}").Uri;

    var deleteResponse = await httpClient.DeleteAsync(deleteUri);

    deleteResponse.EnsureSuccessStatusCode();

    jsonResponse = await httpClient.GetStringAsync(requestUri);

    vendas = JsonSerializer.Deserialize<List<Venda>>(jsonResponse, options);

    Assert.NotNull(vendas);

    vendaASerDeletada = vendas.FirstOrDefault(venda => venda.Id == Guid.Parse(guidString));

    Assert.Null(vendaASerDeletada);
  }
}