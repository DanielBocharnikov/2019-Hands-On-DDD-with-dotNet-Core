using Marketplace.Framework;

namespace Marketplace.Domain;

public interface IClassifiedAdRepository
{
  Task<bool> Exists<T>(string entityId);
  Task<T> Load<T>(string entityId) where T : Entity;
  Task Save<T>(T entity) where T : Entity;
}