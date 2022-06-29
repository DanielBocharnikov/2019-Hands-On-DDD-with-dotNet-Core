using System.Data.Common;
using Marketplace.ClassifiedAd;
using Marketplace.Domain.ClassifiedAd;
using Marketplace.Domain.SharedCore;
using Marketplace.Domain.UserProfile;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Marketplace.UserProfile;
using Npgsql;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// TODO Hide connection string into configuration.
const string ConnectionString = "Host=localhost;Port=5432;Database=Marketplace_Chapter9;Username=ddd;Password=book";

builder.Services.AddEntityFrameworkNpgsql();
builder.Services.AddPostgresDbContext<MarketplaceDbContext>(ConnectionString);
builder.Services.AddScoped<DbConnection>(_ => new NpgsqlConnection(
  ConnectionString));

builder.Services.AddSingleton<ICurrencyLookup, FixedCurrencyLookup>();

builder.Services.AddScoped<IUnitOfWork, EfCoreUnitOfWork>();

builder.Services.AddScoped<IClassifiedAdRepository, ClassifiedAdRepository>();
builder.Services.AddScoped<ClassifiedAdsApplicationService>();

var purgomalumClient = new PurgomalumClient();

builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped(c => new UserProfileApplicationService(
  c.GetService<IUserProfileRepository>()!,
  c.GetService<IUnitOfWork>()!,
  text => purgomalumClient.CheckForProfanity(text).GetAwaiter().GetResult()
));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication? app = builder.Build();

if (app.Environment.IsDevelopment())
{
  _ = app.UseSwagger();
  _ = app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
  });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.EnsureDatabase();

app.Run();
