using System.Data.Common;
using Marketplace.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Marketplace.ClassifiedAd;

[ApiController]
[Route("/ad")]
public class ClassifiedAdsQueryApi : ControllerBase
{
  private static readonly Serilog.ILogger _log
    = Log.ForContext<ClassifiedAdsQueryApi>();

  private readonly DbConnection _connection;

  public ClassifiedAdsQueryApi(DbConnection connection)
    => _connection = connection;

  [HttpGet]
  [Route("list")]
  public Task<IActionResult> Get(
    [FromQuery] QueryModels.GetPublishedClassifiedAds request)
      => RequestHandler.HandleQuery(() => _connection.Query(request), _log);

  [HttpGet]
  [Route("myads")]
  public Task<IActionResult> Get(
    [FromQuery] QueryModels.GetOwnersClassifiedAd request)
      => RequestHandler.HandleQuery(() => _connection.Query(request), _log);

  [HttpGet]
  public Task<IActionResult> Get(
    [FromQuery] QueryModels.GetPublicClassifiedAd request)
      => RequestHandler.HandleQuery(() => _connection.Query(request), _log);
}
