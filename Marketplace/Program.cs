using Marketplace.ClassifiedAd;
using Marketplace.Domain.SharedCore;
using Marketplace.Domain.UserProfile;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Marketplace.UserProfile;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var purgomalumClient = new PurgomalumClient();
builder.Services.AddSingleton<ICurrencyLookup, FixedCurrencyLookup>();
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
  _ = app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
  });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
