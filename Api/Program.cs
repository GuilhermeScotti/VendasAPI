using Data.Repositories;
using DataAbstraction.Repositories;
using Domain.Entidades;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IVendaRepository, VendaRepository>();
builder.Services.AddScoped<IRepository<Venda>, VendaRepository>();
builder.Services.AddScoped<IVendaProdutoRepository, VendaProdutoRepository>();
builder.Services.AddScoped<IRepository<VendaProduto>, VendaProdutoRepository>();
builder.Services.AddScoped<IReadOnlyRepository<Produto>, ProdutoRepository>();
builder.Services.AddScoped<INumeroVendaRepository, NumeroVendaRepository>();

builder.Services.AddSingleton<IServiçoDeMensageria, ServiçoDeMensageria>();

var app = builder.Build();

app.UseExceptionHandler("/error");

app.Map("/error", (HttpContext httpContext) =>
{
  var exception = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

  if (exception != null)
  {
    Log.Error(exception, "Erro capturado em /error");
  }
  else
  {
    Log.Error("Erro desconhecido.");
  }

  var mensagem = exception?.Message ?? "desconhecido";

  return Results.Problem($"Houve um erro na operação: {mensagem}", statusCode: 500);
});

app.MapGet("/", () => "Bem-vindo à API de vendas!");

app.MapGet("/vendas", async (IRepository<Venda> repo) =>
{
  var vendas = await repo.ObterTodosAsync();
  return Results.Ok(vendas);
})
.WithName("ObterTodosVendas");

app.MapGet("/vendas/{id}", async (Guid id, IRepository<Venda> repo) =>
{
  var venda = await repo.ObterPorIdAsync(id);
  return venda is not null ? Results.Ok(venda) : Results.NotFound();
})
.WithName("ObterVendaPorId");

app.MapGet("/vendas/{id}/detalhes", async (Guid id, IVendaRepository repo) =>
{
  var venda = await repo.ObterCompletaPorIdAsync(id);
  return venda is not null ? Results.Ok(venda) : Results.NotFound();
})
.WithName("ObterVendaCompletaPorId");

app.MapPost("/vendas", async (CriarVendaDto novaVenda, IVendaRepository repo, IServiçoDeMensageria mensageria) =>
{
  var vendaCriada = await repo.AdicionarDeDtoAsync(novaVenda);

  await mensageria.VendaCriada(vendaCriada.Id);
  Log.Information("Venda criada com sucesso: {VendaId}", vendaCriada.Id);
  return Results.Created($"/vendas/{vendaCriada.Id}", vendaCriada);
})
.WithName("CriarVenda");

app.MapPatch("/vendas/{id}/cancelar", async (Guid id, CancelarVendaDto atualizarVendaDto, IRepository<Venda> repo, IServiçoDeMensageria mensageria) =>
{
  var venda = await repo.ObterPorIdAsync(id);

  if (venda is null)
  {
    Log.Warning("Cancelando Venda com ID {VendaId} não encontrada.", id);
    return Results.NotFound();
  }

  if (venda.Cancelado)
  {
    Log.Warning("Tentativa de cancelar a venda com ID {VendaId} que já está cancelada.", id);
    return Results.Problem("Venda já cancelada.");
  }

  await repo.AtualizarAsync(venda with
  {
    Cancelado = true,
    MotivoCancelamento = atualizarVendaDto.MotivoCancelamento
  });

  await mensageria.VendaCancelada(venda.Id);

  Log.Information("Venda com ID {VendaId} cancelada com sucesso.", id);

  return Results.NoContent();
})
.WithName("CancelarVenda");

app.MapPatch("/vendas/{id}/fechar", async (Guid id, CancelarVendaDto atualizarVendaDto, IRepository<Venda> repo, IServiçoDeMensageria mensageria) =>
{
  var venda = await repo.ObterPorIdAsync(id);

  if (venda is null)
  {
    Log.Warning("Cancelando Venda com ID {VendaId} não encontrada.", id);
    return Results.NotFound();
  }

  if (venda.Cancelado)
  {
    Log.Warning("Tentativa de cancelar a venda com ID {VendaId} que já está cancelada.", id);
    return Results.Problem("Venda já cancelada.");
  }

  if (venda.Fechada)
  {
    Log.Warning("Tentativa de fechar a venda com ID {VendaId} que já está fechada.", id);
    return Results.Problem("Venda já fechada.");
  }

  await repo.AtualizarAsync(venda with
  {
    Fechada = true
  });

  await mensageria.VendaCancelada(venda.Id);

  Log.Information("Venda com ID {VendaId} cancelada com sucesso.", id);

  return Results.NoContent();
})
.WithName("FecharVenda");

app.MapPost("/vendas/{id}/produtos", async (Guid id, VenderProdutosDto venderProdutosDto,
  IVendaProdutoRepository repo,
  IRepository<Venda> vendaRepo,
  IServiçoDeMensageria mensageria) =>
{
  var venda = await vendaRepo.ObterPorIdAsync(id);

  if (venda is null)
  {
    Log.Warning("Adicionando produtos em Venda com ID {VendaId} não encontrada.", id);
    return Results.NotFound();
  }

  if (venda.Cancelado)
  {
    Log.Warning("Tentativa de adicionar produtos à venda com ID {VendaId} que já está cancelada.", id);
    return Results.Problem("Venda cancelada. Não é possível adicionar produtos.");
  }

  await repo.VenderProdutosAsync(id, venderProdutosDto);

  await mensageria.VendaAlterada(venda.Id);
  Log.Information("Adicionando produtos à venda com ID {VendaId}", id);
  return Results.NoContent();
})
.WithName("AdicionarProdutosVenda");

app.MapPatch("/vendas/cancelarVendaProduto/{id}", async (
  Guid id,
  IVendaProdutoRepository repo,
  IRepository<Venda> vendaRepository,
  IServiçoDeMensageria mensageria) =>
{
  var vendaProduto = await repo.ObterPorIdAsync(id);

  if (vendaProduto is null)
  {
    Log.Warning("Tentativa de cancelar Produto com ID {ProdutoId} não encontrado.", id);
    return Results.NotFound();
  }

  var venda = await vendaRepository.ObterPorIdAsync(vendaProduto.IdVenda);

  if (venda?.Cancelado ?? false)
  {
    Log.Warning("Tentativa de cancelar o produto com ID {ProdutoId} em uma venda que já está cancelada.", id);
    return Results.Problem("Venda cancelada. Não é possível cancelar produto.");
  }

  await repo.AtualizarAsync(vendaProduto with
  {
    Cancelado = true,
  });

  await mensageria.ItemCancelado(vendaProduto.Id);
  Log.Information("Produto com ID {ProdutoId} cancelado com sucesso.", id);
  return Results.NoContent();
})
.WithName("CancelarVendaProduto");

app.MapDelete("/vendas/{id}", async (Guid id, IRepository<Venda> repo, IServiçoDeMensageria mensageria) =>
{
  var deleted = await repo.DeletarAsync(id);

  if (deleted)
  {
    await mensageria.VendaExcluida(id);
    Log.Information("Venda com ID {VendaId} excluída com sucesso.", id);
    return Results.NoContent();
  }

  Log.Warning("Não foi possível excluir a venda com ID {VendaId}.", id);
  return Results.Problem("Não foi possível excluir venda.");
})
.WithName("DeletarVenda");

app.Run();
