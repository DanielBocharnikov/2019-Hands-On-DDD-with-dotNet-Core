using Marketplace;
using Marketplace.Api;
using Marketplace.Domain;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Raven.Client.Documents;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var store = new DocumentStore
{
  Urls = new[] { "http://localhost:8080" },
  Database = "Marketplace_Chapter8",
  Conventions =
  {
    FindIdentityProperty = m => m.Name == "DbId"
  }
};

store.Initialize();

builder.Services.AddSingleton<ICurrencyLookup, FixedCurrencyLookup>();
builder.Services.AddScoped(_ => store.OpenAsyncSession());
builder.Services.AddScoped<IUnitOfWork, RavenDbUnitOfWork>();
builder.Services.AddScoped<IClassifiedAdRepository, ClassifiedAdRepository>();
builder.Services.AddScoped<ClassifiedAdsApplicationService>();

WebApplication? app = builder.Build();

if (app.Environment.IsDevelopment())
{
  _ = app.UseSwagger();
  _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
