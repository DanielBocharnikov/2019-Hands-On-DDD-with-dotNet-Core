using Marketplace.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;
using Serilog;

namespace Marketplace.ClassifiedAd;

[ApiController]
[Route("/ad")]
public class ClassifiedAdsQueryApi : ControllerBase
{
  private static readonly Serilog.ILogger _log
    = Log.ForContext<ClassifiedAdsQueryApi>();

  private readonly IAsyncDocumentSession _session;

  public ClassifiedAdsQueryApi(IAsyncDocumentSession session)
    => _session = session;

  [HttpGet]
  [Route("list")]
  public async Task<IActionResult> Get(
    [FromQuery] QueryModels.GetPublishedClassifiedAds request)
      => await RequestHandler.HandleQuery(() => _session.Query(request), _log);

  [HttpGet]
  [Route("myads")]
  public async Task<IActionResult> Get(
    [FromQuery] QueryModels.GetOwnersClassifiedAd request)
      => await RequestHandler.HandleQuery(() => _session.Query(request), _log);

  [HttpGet]
  public async Task<IActionResult> Get(
    [FromQuery] QueryModels.GetPublicClassifiedAd request)
      => await RequestHandler.HandleQuery(() => _session.Query(request), _log);
}