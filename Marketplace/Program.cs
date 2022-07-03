using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Marketplace;
using Marketplace.ClassifiedAd;
using Marketplace.Infrastructure;
using Marketplace.UserProfile;
using static System.Reflection.Assembly;

string currentDirectory = Path.GetDirectoryName(
   GetEntryAssembly()?.Location)!;

// IConfigurationRoot config = new ConfigurationBuilder()
//   .SetBasePath(currentDirectory)
//   .AddJsonFile("appsettings.json", optional: false,
//     reloadOnChange: false)
//   .Build();

WebApplicationBuilder? builder = WebApplication.CreateBuilder(
  new WebApplicationOptions
  {
    Args = args,
    ContentRootPath = currentDirectory,
  }
);
{
  // _ = builder.WebHost.UseConfiguration(config);

  _ = builder.Services.AddControllers();
  _ = builder.Services.AddEndpointsApiExplorer();
  _ = builder.Services.AddSwaggerGen();

  var credentials = new UserCredentials(
    builder.Configuration["EventStore:UserName"],
    builder.Configuration["EventStore:Password"]
  );

  ConnectionSettings connectionSettings = ConnectionSettings
    .Create()
    .SetDefaultUserCredentials(credentials)
    .DisableTls()
    .KeepReconnecting()
    .SetHeartbeatTimeout(TimeSpan.FromMilliseconds(500))
    .Build();

  IEventStoreConnection esConnection = EventStoreConnection.Create(
    connectionSettings,
    new Uri(builder.Configuration["EventStore:Url"]),
    builder.Configuration["EventStore:ConnectionName"]
  );

  EsAggregateStore store = new(esConnection);

  _ = builder.Services.AddSingleton(esConnection);
  _ = builder.Services.AddSingleton(store);

  _ = builder.Services.AddSingleton(new ClassifiedAdsApplicationService(
    store, new FixedCurrencyLookup()));

  PurgomalumClient purgomalumClient = new();

  _ = builder.Services.AddSingleton(new UserProfileApplicationService(
    store,
    text => purgomalumClient.CheckForProfanity(text).GetAwaiter().GetResult()));

  _ = builder.Services.AddSingleton<IHostedService, HostedService>();
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