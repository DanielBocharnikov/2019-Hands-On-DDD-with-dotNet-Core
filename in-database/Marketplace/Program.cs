using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Marketplace;
using Marketplace.ClassifiedAd;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Marketplace.Projections;
using Marketplace.UserProfile;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.ServerWide.Operations;
using Serilog;
using static System.Reflection.Assembly;

string currentDirectory = Path.GetDirectoryName(
   GetEntryAssembly()?.Location)!;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(
  new WebApplicationOptions
  {
    Args = args,
    ContentRootPath = currentDirectory,
  }
);
{
  _ = builder.Services.AddControllers();
  _ = builder.Services.AddEndpointsApiExplorer();
  _ = builder.Services.AddSwaggerGen();

  ConnectionSettings connectionSettings = ConnectionSettings
    .Create()
    .SetDefaultUserCredentials(
      new UserCredentials(
        builder.Configuration["EventStore:UserName"],
        builder.Configuration["EventStore:Password"]
      )
    )
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
  _ = builder.Services.AddSingleton<IAggregateStore>(store);
  _ = builder.Services.AddSingleton(
    new ClassifiedAdsApplicationService(store, new FixedCurrencyLookup())
  );

  PurgomalumClient purgomalumClient = new();
  _ = builder.Services.AddSingleton(
    new UserProfileApplicationService(
      store,
      text => purgomalumClient.CheckForProfanity(text).GetAwaiter().GetResult()
    )
  );

  IDocumentStore documentStore = ConfigureRavenDb(
    builder.Configuration.GetSection("ravenDb")
  );

  Func<IAsyncDocumentSession> getSession =
    () => documentStore.OpenAsyncSession();
  _ = builder.Services.AddTransient(_ => getSession());

  ProjectionManager projectionManager = new(
    esConnection,
    new RavenDbCheckpointStore(getSession, "readmodels"),
    new UserProfileDetailsProjection(getSession),
    new ClassifiedAdDetailsProjection(
      getSession,
      async userId => (await getSession.GetUserDetails(userId)).DisplayName
    ),
    new ClassifiedAdUpcasters(
      esConnection,
      async userId => (await getSession.GetUserDetails(userId)).PhotoUrl
    )
  );

  _ = builder.Services.AddSingleton<IHostedService>(
    new EventStoreService(esConnection, projectionManager)
  );

  _ = builder.Host.UseSerilog((_, lc) => lc
  .MinimumLevel.Debug()
  .WriteTo.Console());
}

static IDocumentStore ConfigureRavenDb(
  IConfigurationSection configurationSection
)
{
  DocumentStore store = new()
  {
    Urls = new[]
    {
      configurationSection["server"]
    },
    Database = configurationSection["database"]
  };

  _ = store.Initialize();

  Raven.Client.ServerWide.DatabaseRecordWithEtag record = store
    .Maintenance
    .Server
    .Send(new GetDatabaseRecordOperation(store.Database));

  if (record is null)
  {
    _ = store
      .Maintenance
      .Server
      .Send(new GetDatabaseRecordOperation(store.Database));
  }

  return store;
}

try
{
  Log.Information("Starting web host");

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
}
catch (Exception ex)
{
  Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
  Log.CloseAndFlush();
}