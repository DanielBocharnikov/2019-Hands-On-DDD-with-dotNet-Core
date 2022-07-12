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
  public Task<IActionResult> Get(
    [FromQuery] QueryModels.GetPublicClassifiedAd request)
      => RequestHandler.HandleQuery(() => _session.Query(request), _log);
}