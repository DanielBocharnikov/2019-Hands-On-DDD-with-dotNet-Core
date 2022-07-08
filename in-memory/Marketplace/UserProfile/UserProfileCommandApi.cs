using Marketplace.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.UserProfile;

[ApiController]
[Route("/profile")]
public class UserProfileCommandApi : ControllerBase
{
  private readonly UserProfileApplicationService _applicationService;

  private static readonly Serilog.ILogger _log
    = Serilog.Log.ForContext<UserProfileCommandApi>();

  public UserProfileCommandApi(UserProfileApplicationService applicationService)
      => _applicationService = applicationService;

  [HttpPost]
  public Task<IActionResult> Post(Commands.V1.RegisterUser request)
    => RequestHandler.HandleCommand(request, _applicationService.Handle, _log);

  [HttpPut]
  [Route("fullname")]
  public Task<IActionResult> Put(Commands.V1.UpdateUserFullName request)
    => RequestHandler.HandleCommand(request, _applicationService.Handle, _log);

  [HttpPut]
  [Route("displayname")]
  public Task<IActionResult> Put(Commands.V1.UpdateUserDisplayName request)
    => RequestHandler.HandleCommand(request, _applicationService.Handle, _log);

  [HttpPut]
  [Route("photo")]
  public Task<IActionResult> Put(Commands.V1.UpdateUserProfilePhoto request)
    => RequestHandler.HandleCommand(request, _applicationService.Handle, _log);
}