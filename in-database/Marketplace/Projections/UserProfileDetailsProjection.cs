using Marketplace.Domain.UserProfile;
using Marketplace.Infrastructure;
using Raven.Client.Documents.Session;

namespace Marketplace.Projections;

public class UserProfileDetailsProjection
  : RavenDbProjection<ReadModels.UserDetails>
{
  public UserProfileDetailsProjection(
    Func<IAsyncDocumentSession> getSession
  ) : base(getSession)
  {
  }

  public override Task Project(object resolvedEvent)
    => resolvedEvent switch
    {
      Events.UserRegistered e => Create(
        () => Task.FromResult(
          new ReadModels.UserDetails
          {
            Id = e.UserId.ToString(),
            DisplayName = e.DisplayName,
            PhotoUrl = string.Empty
          }
        )
      ),
      Events.UserDisplayNameUpdated e => UpdateOne(
        e.UserId,
        u => u.DisplayName = e.DisplayName
      ),
      Events.ProfilePhotoUploaded e => UpdateOne(
        e.UserId,
        u => u.PhotoUrl = e.PhotoUrl
      ),
      _ => Task.CompletedTask
    };
}