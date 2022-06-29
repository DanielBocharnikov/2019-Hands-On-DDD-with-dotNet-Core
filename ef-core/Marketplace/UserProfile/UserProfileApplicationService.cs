using Marketplace.Domain.SharedCore;
using Marketplace.Domain.UserProfile;
using Marketplace.Framework;

namespace Marketplace.UserProfile;

public class UserProfileApplicationService : IApplicationService
{
  private readonly IUserProfileRepository _repository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly CheckTextForProfanity _checkText;

  public UserProfileApplicationService(IUserProfileRepository repository, IUnitOfWork unitOfWork, CheckTextForProfanity checkText)
  {
    _repository = repository;
    _unitOfWork = unitOfWork;
    _checkText = checkText;
  }

  public async Task Handle(object command)
  {
    switch (command)
    {
      case Commands.V1.RegisterUser cmd:
        if (await _repository.Exists((UserId)cmd.UserId))
        {
          throw new InvalidOperationException(
            $"Entity with id {cmd.UserId} already exists"
          );
        }

        var userProfile = new Domain.UserProfile.UserProfile(
          new UserId(cmd.UserId),
          FullName.FromString(cmd.FullName),
          DisplayName.FromString(cmd.DisplayName, _checkText)
        );

        await _repository.Add(userProfile);
        await _unitOfWork.Commit();
        break;
      case Commands.V1.UpdateUserFullName cmd:
        await HandleUpdate(cmd.UserId, profile =>
          profile.UpdateFullName(FullName.FromString(cmd.FullName)));
        break;
      case Commands.V1.UpdateUserDisplayName cmd:
        await HandleUpdate(cmd.UserId, profile =>
          profile.UpdateDisplayName(
            DisplayName.FromString(cmd.DisplayName, _checkText)
          )
        );
        break;
      case Commands.V1.UpdateUserProfilePhoto cmd:
        await HandleUpdate(cmd.UserId, profile =>
          profile.UpdateProfilePhoto(new Uri(cmd.PhotoUrl))
        );
        break;
      default:
        throw new InvalidOperationException(
          $"Command type {command.GetType().FullName} is unknown."
        );
    }
  }

  private async Task HandleUpdate(
    Guid userProfileId,
    Action<Domain.UserProfile.UserProfile> operation
  )
  {
    Domain.UserProfile.UserProfile? userProfile = await _repository
      .Load((UserId)userProfileId) ?? throw new InvalidOperationException(
        $"Entity with id {userProfileId} cannot be found.");

    operation(userProfile);

    await _unitOfWork.Commit();
  }
}