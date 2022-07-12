using Raven.Client.Documents.Session;
using static Marketplace.Projections.ReadModels;

namespace Marketplace.UserProfile;

public static class Queries
{
  public static async Task<UserDetails> GetUserDetails(
    this Func<IAsyncDocumentSession> getSession,
    Guid id
  )
  {
    using IAsyncDocumentSession session = getSession();

    return await session.LoadAsync<UserDetails>(id.ToString());
  }
}