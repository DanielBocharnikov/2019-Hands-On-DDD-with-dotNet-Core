using EventStore.ClientAPI;
using Marketplace;
using Marketplace.ClassifiedAd;
using Marketplace.Domain.SharedCore;
using Marketplace.Domain.UserProfile;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Marketplace.UserProfile;
using static System.Reflection.Assembly;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
{
  // string currentDirectory = Path.GetDirectoryName(
  //  GetEntryAssembly()?.Location)!;

  // IConfigurationRoot config = new ConfigurationBuilder()
  //   .SetBasePath(currentDirectory)
  //   .AddJsonFile("appsettings.development.json", optional: false,
  //     reloadOnChange: false)
  //   .Build();

  // _ = builder.WebHost
  //   .UseConfiguration(config)
  //   .UseContentRoot(currentDirectory);

  _ = builder.Services.AddControllers();
  _ = builder.Services.AddEndpointsApiExplorer();
  _ = builder.Services.AddSwaggerGen();

  IEventStoreConnection esConnection = EventStoreConnection.Create(
    builder.Configuration["eventStore:connectionString"],
    ConnectionSettings.Create().KeepReconnecting(),
    builder.Environment.ApplicationName
  );

  EsAggregateStore store = new(esConnection);

  _ = builder.Services.AddSingleton(esConnection);
  _ = builder.Services.AddSingleton(store);

  _ = builder.Services.AddSingleton(new ClassifiedAdsApplicationService(
    store, new FixedCurrencyLookup()));

  PurgomalumClient purgomalumClient = new();

  _ = builder.Services.AddSingleton(new UserProfileApplicationService(
    store, text => purgomalumClient.CheckForProfanity(text)));

  _ = builder.Services
    .AddSingleton<BackgroundService, EventStoreConnectionService>();
}

WebApplication? app = builder.Build();
{
  if (app.Environment.IsDevelopment())
  {
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI(options =>
    {
      options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
      options.RoutePrefix = string.Empty;
    });
  }

  _ = app.UseHttpsRedirection();

  _ = app.UseAuthorization();

  _ = app.MapControllers();

  app.Run();
}