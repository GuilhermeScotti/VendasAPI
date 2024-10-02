using Data.Repositories;
using DataAbstraction.Repositories;
using Domain.Entidades;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IRepository<Venda>, VendaRepository>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

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


app.Run();
