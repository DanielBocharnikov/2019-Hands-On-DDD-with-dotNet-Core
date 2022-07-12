using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Marketplace;
using Marketplace.ClassifiedAd;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Marketplace.Projections;
using Marketplace.UserProfile;
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
  _ = builder.Host.UseSerilog((_, lc) => lc
    .MinimumLevel.Debug()
    .WriteTo.Console());

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
  _ = builder.Services.AddSingleton<IAggregateStore>(store);

  _ = builder.Services.AddSingleton(new ClassifiedAdsApplicationService(
    store, new FixedCurrencyLookup()));

  PurgomalumClient purgomalumClient = new();

  _ = builder.Services.AddSingleton(new UserProfileApplicationService(
    store,
    text => purgomalumClient.CheckForProfanity(text).GetAwaiter().GetResult()));

  List<ReadModels.ClassifiedAdDetails> classifiedAdDetails = new();
  _ = builder.Services
    .AddSingleton<IEnumerable<
      ReadModels.ClassifiedAdDetails>>(classifiedAdDetails);

  List<ReadModels.UserDetails> userDetails = new();
  _ = builder.Services
    .AddSingleton<IEnumerable<ReadModels.UserDetails>>(userDetails);

  var projectionManager = new ProjectionManager(esConnection,
    new UserProfileDetailsProjection(userDetails),
    new ClassifiedAdDetailsProjection(classifiedAdDetails,
      userId => userDetails.Find(x => x.UserId == userId)?.DisplayName
        ?? string.Empty),
    new ClassifiedAdUpcasters(esConnection,
      userId => userDetails.Find(x => x.UserId == userId)?.PhotoUrl
        ?? string.Empty));

  _ = builder.Services.AddSingleton<IHostedService>(
    new EventStoreService(esConnection, projectionManager)
  );
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