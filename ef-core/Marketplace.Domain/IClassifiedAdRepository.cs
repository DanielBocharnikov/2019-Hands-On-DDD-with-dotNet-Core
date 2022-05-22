namespace Marketplace.Domain;

public interface IClassifiedAdRepository
{
  Task<bool> Exists(ClassifiedAdId entityId);
  Task<ClassifiedAd?> Load(ClassifiedAdId entityId);
  Task Add(ClassifiedAd entity);
}