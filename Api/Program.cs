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

var app = builder.Build();

app.UseExceptionHandler("/error");

app.Map("/error", (HttpContext httpContext) =>
{
  var exception = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

  // Logar exception

  return Results.Problem("Houve um erro na operação.", statusCode: 500);
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

app.MapGet("/vendaCompleta/{id}", async (Guid id, IVendaRepository repo) =>
{
  var venda = await repo.ObterCompletaPorIdAsync(id);
  return venda is not null ? Results.Ok(venda) : Results.NotFound();
})
.WithName("ObterVendaCompletaPorId");

app.MapPost("/vendas", async (CriarVendaDto novaVenda, IVendaRepository repo) =>
{
  var vendaCriada = await repo.AdicionarDeDto(novaVenda);
  return Results.Created($"/vendas/{vendaCriada.Id}", vendaCriada);
})
.WithName("CriarVenda");

app.MapPut("/vendas/{id}", async (Guid id, Venda vendaAtualizada, IRepository<Venda> repo) =>
{
  var venda = await repo.ObterPorIdAsync(id);

  if (venda is null) return Results.NotFound();

  await repo.AtualizarAsync(vendaAtualizada);
  return Results.NoContent();
})
.WithName("AtualizarVenda");

app.MapDelete("/vendas/{id}", async (Guid id, IRepository<Venda> repo) =>
{
  var deleted = await repo.DeletarAsync(id);
  return deleted ? Results.NoContent() : Results.NotFound();
})
.WithName("DeletarVenda");

app.Run();
