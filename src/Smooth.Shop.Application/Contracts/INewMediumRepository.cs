using Smooth.Shop.Domain.Models;

namespace Smooth.Shop.Application.Contracts;

public interface INewMediumRepository
{
    Task<NewMedium?> GetByIdAsync(long id);

    Task<IEnumerable<NewMedium>> GetAllAsync();

    Task AddAsync(NewMedium newMedium);

    Task DeleteAsync(long id);
}
