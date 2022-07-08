using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Infrastructure;

public static class RequestHandler
{
  public static async Task<IActionResult> HandleCommand<T>(
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

  public static IActionResult HandleQuery<TModel>(
    Func<TModel> query, Serilog.ILogger log
  )
  {
    try
    {
      return new OkObjectResult(query());
    }
    catch (Exception ex)
    {
      log.Error(ex, "Error handling query");

      return new BadRequestObjectResult(new
      {
        error = ex.Message,
        stackTrace = ex.StackTrace
      });
    }
  }
}