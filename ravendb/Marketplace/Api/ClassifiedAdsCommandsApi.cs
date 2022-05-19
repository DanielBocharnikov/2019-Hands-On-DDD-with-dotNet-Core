using Microsoft.AspNetCore.Mvc;
using static Marketplace.Contracts.ClassifiedAds;

namespace Marketplace.Api;

[Route("/ad")]
[ApiController]
public class ClassifiedAdsCommandsApi : ControllerBase
{
  private readonly ClassifiedAdsApplicationService _appService;

  public ClassifiedAdsCommandsApi(ClassifiedAdsApplicationService appService)
    => _appService = appService;

  [HttpPost]
  public async Task<IActionResult> Post(V1.Create request)
  {
    await _appService.Handle(request);
    return Ok();
  }

  [Route("name")]
  [HttpPut]
  public async Task<IActionResult> Put(V1.SetTitle request)
  {
    await _appService.Handle(request);
    return Ok();
  }

  [Route("text")]
  [HttpPut]
  public async Task<IActionResult> Put(V1.UpdateText request)
  {
    await _appService.Handle(request);
    return Ok();
  }

  [Route("price")]
  [HttpPut]
  public async Task<IActionResult> Put(V1.UpdatePrice request)
  {
    await _appService.Handle(request);
    return Ok();
  }

  [Route("publish")]
  [HttpPut]
  public async Task<IActionResult> Put(V1.RequestToPublish request)
  {
    await _appService.Handle(request);
    return Ok();
  }
}