using Data.Repositories;
using DataAbstraction.Repositories;
using Domain.Entidades;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IVendaRepository, VendaRepository>();
builder.Services.AddScoped<IRepository<Venda>, VendaRepository>();
builder.Services.AddScoped<IVendaProdutoRepository, VendaProdutoRepository>();
builder.Services.AddScoped<IRepository<VendaProduto>, VendaProdutoRepository>();
builder.Services.AddScoped<IRepository<Produto>, ProdutoRepository>();
builder.Services.AddScoped<INumeroVendaRepository, NumeroVendaRepository>();

builder.Services.AddSingleton<IServiçoDeMensageria, ServiçoDeMensageria>();

var app = builder.Build();

app.UseExceptionHandler("/error");

app.Map("/error", (HttpContext httpContext) =>
{
  var exception = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

  // Logar exception

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

  await mensageria.CompraCriada();
  return Results.Created($"/vendas/{vendaCriada.Id}", vendaCriada);
})
.WithName("CriarVenda");

app.MapPatch("/vendas/{id}/cancelar", async (Guid id, CancelarVendaDto atualizarVendaDto, IRepository<Venda> repo, IServiçoDeMensageria mensageria) =>
{
  var venda = await repo.ObterPorIdAsync(id);

  if (venda is null)
    return Results.NotFound();

  if (venda.Cancelado)
    return Results.Problem("Venda já cancelada.");

  await repo.AtualizarAsync(venda with
  {
    Cancelado = true,
    MotivoCancelamento = atualizarVendaDto.MotivoCancelamento
  });

  await mensageria.CompraCancelada();
  return Results.NoContent();
})
.WithName("CancelarVenda");

app.MapPost("/vendas/{id}/produtos", async (Guid id, VenderProdutosDto venderProdutosDto,
  IVendaProdutoRepository repo,
  IRepository<Venda> vendaRepo,
  IServiçoDeMensageria mensageria) =>
{
  var venda = await vendaRepo.ObterPorIdAsync(id);

  if (venda is null)
    return Results.NotFound();

  if (venda.Cancelado)
    return Results.Problem("Venda cancelada. Não é possível adicionar produtos.");

  await repo.VenderProdutosAsync(id, venderProdutosDto);

  await mensageria.CompraAlterada();
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

  if (vendaProduto is null) return Results.NotFound();

  var venda = await vendaRepository.ObterPorIdAsync(vendaProduto.IdVenda);

  if (venda?.Cancelado ?? false)
    return Results.Problem("Venda cancelada. Não é possível cancelar produto.");

  await repo.AtualizarAsync(vendaProduto with
  {
    Cancelado = true,
  });

  await mensageria.ItemCancelado();
  return Results.NoContent();
})
.WithName("CancelarVendaProduto");

app.MapDelete("/vendas/{id}", async (Guid id, IRepository<Venda> repo, IServiçoDeMensageria mensageria) =>
{
  var deleted = await repo.DeletarAsync(id);

  if (deleted)
    await mensageria.CompraExcluida();
  else
    return Results.Problem("Não foi possível excluir venda.");

  return deleted ? Results.NoContent() : Results.NotFound();
})
.WithName("DeletarVenda");

app.Run();
