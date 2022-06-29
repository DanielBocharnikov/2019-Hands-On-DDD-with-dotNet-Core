using Microsoft.AspNetCore.Mvc;
using Serilog;
using static Marketplace.ClassifiedAd.Commands;
using ILogger = Serilog.ILogger;

namespace Marketplace.ClassifiedAd;

[Route("/ad")]
[ApiController]
public class ClassifiedAdsCommandsApi : ControllerBase
{
  private readonly ClassifiedAdsApplicationService _appService;
  private static readonly ILogger _log =
    Log.ForContext<ClassifiedAdsCommandsApi>();

  public ClassifiedAdsCommandsApi(ClassifiedAdsApplicationService appService)
    => _appService = appService;

  [HttpPost]
  public async Task<IActionResult> Post(V1.Create request) =>
    await HandleRequest(request, _appService.Handle);

  [Route("name")]
  [HttpPut]
  public async Task<IActionResult> Put(V1.SetTitle request) =>
    await HandleRequest(request, _appService.Handle);

  [Route("text")]
  [HttpPut]
  public async Task<IActionResult> Put(V1.UpdateText request) =>
    await HandleRequest(request, _appService.Handle);

  [Route("price")]
  [HttpPut]
  public async Task<IActionResult> Put(V1.UpdatePrice request) =>
    await HandleRequest(request, _appService.Handle);

  [Route("addpicture")]
  [HttpPut]
  public async Task<IActionResult> Put(V1.AddPicture request) =>
    await HandleRequest(request, _appService.Handle);

  [Route("resizepicture")]
  [HttpPut]
  public async Task<IActionResult> Put(V1.ResizePicture request) =>
    await HandleRequest(request, _appService.Handle);

  [Route("requestpublish")]
  [HttpPut]
  public async Task<IActionResult> Put(V1.RequestToPublish request) =>
    await HandleRequest(request, _appService.Handle);

  [Route("publish")]
  [HttpPut]
  public async Task<IActionResult> Put(V1.Publish request) =>
  await HandleRequest(request, _appService.Handle);

  private async Task<IActionResult> HandleRequest<T>(
    T request,
    Func<T, Task> handlerHandle)
  {
    try
    {
      _log.Debug($"Handling HTTP request of type {typeof(T).Name}");

      await handlerHandle(request);

      return Ok();
    }
    catch (Exception ex)
    {
      _log.Error("Error handling the HTTP request", ex);

      return new BadRequestObjectResult(new
      {
        error = ex.Message,
        stackTrace = ex.StackTrace
      });
    }
  }
}