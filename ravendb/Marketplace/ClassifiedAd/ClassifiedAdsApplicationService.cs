using Marketplace.Domain.ClassifiedAd;
using Marketplace.Domain.SharedCore;
using Marketplace.Framework;
using static Marketplace.ClassifiedAd.Commands;

namespace Marketplace.ClassifiedAd;

public class ClassifiedAdsApplicationService : IApplicationService
{
  private readonly IClassifiedAdRepository _repository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly ICurrencyLookup _currencyLookup;

  public ClassifiedAdsApplicationService(
    IClassifiedAdRepository repository,
    IUnitOfWork unitOfWork,
    ICurrencyLookup currencyLookup)
  {
    _repository = repository;
    _unitOfWork = unitOfWork;
    _currencyLookup = currencyLookup;
  }

  public Task Handle(object command) =>
    command switch
    {
      V1.Create cmd => HandleCreate(cmd),

      V1.SetTitle cmd => HandleUpdate(cmd.Id, ad =>
        ad.SetTitle(ClassifiedAdTitle.FromString(cmd.Title))),

      V1.UpdateText cmd => HandleUpdate(cmd.Id, ad =>
        ad.UpdateText(ClassifiedAdText.FromString(cmd.Text))),

      V1.UpdatePrice cmd => HandleUpdate(cmd.Id, ad =>
        ad.UpdatePrice(Price.FromDecimal(cmd.Price, cmd.CurrencyCode,
          _currencyLookup))),

      V1.RequestToPublish cmd => HandleUpdate(cmd.Id, ad =>
        ad.RequestToPublish()),

      V1.Publish cmd => HandleUpdate(cmd.Id, c =>
        c.Publish(new UserId(cmd.ApprovedBy))),

      _ => throw new InvalidOperationException(
          $"Command type {command.GetType().FullName} is unknown."
        ),
    };

  private async Task HandleCreate(V1.Create cmd)
  {
    if (await _repository.Exists(cmd.Id.ToString()))
    {
      throw new InvalidOperationException($"Entity with id {cmd.Id}," +
        " already exists");
    }

    var classifiedAd = new Domain.ClassifiedAd.ClassifiedAd(
      new ClassifiedAdId(cmd.Id),
      new UserId(cmd.OwnerId)
    );

    await _repository.Add(classifiedAd);
    await _unitOfWork.Commit();
  }

  private async Task HandleUpdate(
    Guid classifiedAdId,
    Action<Domain.ClassifiedAd.ClassifiedAd> operation)
  {
    Domain.ClassifiedAd.ClassifiedAd? classifiedAd
      = await _repository.Load(classifiedAdId.ToString());

    if (classifiedAd == null)
    {
      throw new InvalidOperationException($"Entity with id {classifiedAdId}"
        + "cannot be found.");
    }

    operation(classifiedAd);

    await _unitOfWork.Commit();
  }
}