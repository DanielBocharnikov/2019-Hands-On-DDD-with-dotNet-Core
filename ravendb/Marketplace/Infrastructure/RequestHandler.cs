using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Infrastructure;

public static class RequestHandler
{
  public static async Task<IActionResult> HandleRequest<T>(
    T request,
    Func<T, Task> handlerHandle,
    Serilog.ILogger log)
  {
    try
    {
      log.Debug("Handling HTTP request of type {Type}", typeof(T).Name);
      await handlerHandle(request);
      return new OkResult();
    }
    catch (Exception ex)
    {
      log.Error(ex, "Error handling the request");

      return new BadRequestObjectResult(new
      {
        error = ex.Message,
        stackTrace = ex.StackTrace
      });
    }
  }
}