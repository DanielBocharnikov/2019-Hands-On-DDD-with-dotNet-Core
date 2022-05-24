using Marketplace.ClassifiedAd;
using Marketplace.Domain.ClassifiedAd;
using Marketplace.Domain.SharedCore;
using Marketplace.Domain.UserProfile;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Marketplace.UserProfile;
using Raven.Client.Documents;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var store = new DocumentStore
{
  Urls = new[] { "http://localhost:8080" },
  Database = "Marketplace_Chapter9",
  Conventions =
  {
    FindIdentityProperty = m => m.Name == "DbId"
  }
};

store.Initialize();

var purgomalumClient = new PurgomalumClient();
builder.Services.AddSingleton<ICurrencyLookup, FixedCurrencyLookup>();
builder.Services.AddScoped(_ => store.OpenAsyncSession());
builder.Services.AddScoped<IUnitOfWork, RavenDbUnitOfWork>();
builder.Services.AddScoped<IClassifiedAdRepository, ClassifiedAdRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<ClassifiedAdsApplicationService>();
builder.Services.AddScoped(c => new UserProfileApplicationService(
  c.GetService<IUserProfileRepository>()!,
  c.GetService<IUnitOfWork>()!,
  text => purgomalumClient.CheckForProfanity(text).GetAwaiter().GetResult()
));

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
