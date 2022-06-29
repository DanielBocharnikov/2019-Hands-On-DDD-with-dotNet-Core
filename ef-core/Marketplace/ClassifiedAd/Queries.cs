using System.Data.Common;
using Dapper;
using static Marketplace.ClassifiedAd.QueryModels;
using static Marketplace.ClassifiedAd.ReadModels;
using static Marketplace.Domain.ClassifiedAd.ClassifiedAd;

namespace Marketplace.ClassifiedAd;

public static class Queries
{
  public static Task<IEnumerable<PublicClassifiedAdListItem>> Query(
    this DbConnection connection,
    GetPublishedClassifiedAds query
  ) => connection.QueryAsync<PublicClassifiedAdListItem>(
    @"
      SELECT
        c.""ClassifiedAdId"",
        c.""Title_Value"" title,
        c.""Price_Amount"" price,
        c.""Price_Currency_CurrencyCode"" currencyCode,
        p.""Location"" photoUrl
      FROM ""ClassifiedAds"" c
      INNER JOIN ""Pictures"" p
      ON p.""OwnerClassifiedAdId"" = c.""ClassifiedAdId""
      WHERE c.""State""=@State
      LIMIT @PageSize
      OFFSET @Offset
    ",
    new
    {
      State = (int)ClassifiedAdState.Active,
      query.PageSize,
      Offset = Offset(query.Page, query.PageSize)
    }
  );

  public static Task<IEnumerable<PublicClassifiedAdListItem>> Query(
    this DbConnection connection,
    GetOwnersClassifiedAd query
  ) => connection.QueryAsync<PublicClassifiedAdListItem>(
    @"
      SELECT
        c.""ClassifiedAdId"",
        c.""Title_Value"" title,
        c.""Price_Amount"" price,
        c.""Price_Currency_CurrencyCode"" currencyCode,
        p.""Location"" photoUrl
      FROM ""ClassifiedAds"" c
      INNER JOIN ""Pictures"" p
      ON p.""OwnerClassifiedAdId"" = c.""ClassifiedAdId""
      WHERE c.""OwnerId_Value""=@OwnerId
      LIMIT @PageSize
      OFFSET @Offset
    ",
    new
    {
      query.OwnerId,
      query.PageSize,
      Offset = Offset(query.Page, query.PageSize)
    }
  );

  private static int Offset(int page, int pageSize) => page * pageSize;

  public static Task<ClassifiedAdDetails> Query(
    this DbConnection connection,
    GetPublicClassifiedAd query
  ) => connection.QuerySingleOrDefaultAsync<ClassifiedAdDetails>(
    @"
      SELECT
        c.""ClassifiedAdId"",
        c.""Title_Value"" title,
        c.""Price_Amount"" price,
        c.""Price_Currency_CurrencyCode"" currencyCode,
        u.""DisplayName_Value"" sellerDisplayName,
        c.""Text_Value"" description,
        (SELECT array_agg(p.""Location"")
        FROM ""Pictures"" p
        WHERE p.""OwnerClassifiedAdId"" = c.""ClassifiedAdId"") AS photoUrls
      FROM ""ClassifiedAds"" c
      JOIN ""UserProfiles"" u
      ON u.""UserProfileId"" = c.""OwnerId_Value""
      WHERE ""ClassifiedAdId"" = @Id
    ",
    new
    {
      Id = query.ClassifiedAdId
    }
  );
}