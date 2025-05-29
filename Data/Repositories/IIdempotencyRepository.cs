using DermatologyApi.Models;

namespace DermatologyApi.Data.Repositories
{
    public interface IIdempotencyRepository
    {
        Task<IdempotencyRecord?> GetByKeyAsync(string key);
        Task CreateAsync(IdempotencyRecord record);
        Task UpdateResultAsync(string key, string result, string status);
        Task UpdateStatusAsync(string key, string status, string? errorMessage = null);
    }
}
