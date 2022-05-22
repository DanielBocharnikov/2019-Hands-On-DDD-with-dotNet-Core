using Marketplace;
using Marketplace.Api;
using Marketplace.Domain;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// TODO: Hide connection string into configuration.
const string ConnectionString = "Host=localhost;Port=5432;Database=Marketplace_Chapter8;Username=ddd;Password=book";

builder
  .Services
  .AddEntityFrameworkNpgsql()
  .AddDbContext<ClassifiedAdDbContext>(options => options.UseNpgsql(ConnectionString));

builder.Services.AddSingleton<ICurrencyLookup, FixedCurrencyLookup>();

builder.Services.AddScoped<IUnitOfWork, EfCoreUnitOfWork>();
builder.Services.AddScoped<IClassifiedAdRepository, ClassifiedAdRepository>();
builder.Services.AddScoped<ClassifiedAdsApplicationService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication? app = builder.Build();

if (app.Environment.IsDevelopment())
{
  _ = app.UseSwagger();
  _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.EnsureDatabase();

app.MapControllers();

app.Run();
