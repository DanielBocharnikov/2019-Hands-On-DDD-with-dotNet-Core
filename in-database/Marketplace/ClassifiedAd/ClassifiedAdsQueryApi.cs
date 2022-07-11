using Marketplace.Infrastructure;
using Marketplace.Projections;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Marketplace.ClassifiedAd;

[ApiController]
[Route("/ad")]
public class ClassifiedAdsQueryApi : ControllerBase
{
  private static readonly Serilog.ILogger _log
    = Log.ForContext<ClassifiedAdsQueryApi>();

  private readonly IEnumerable<ReadModels.ClassifiedAdDetails> _items;

  public ClassifiedAdsQueryApi(
    IEnumerable<ReadModels.ClassifiedAdDetails> items) => _items = items;

  [HttpGet]
  public IActionResult Get(
    [FromQuery] QueryModels.GetPublicClassifiedAd request)
      => RequestHandler.HandleQuery(() => _items.Query(request), _log);
}