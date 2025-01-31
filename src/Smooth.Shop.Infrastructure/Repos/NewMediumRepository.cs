using Microsoft.EntityFrameworkCore;
using Smooth.Shop.Application.Contracts;
using Smooth.Shop.Domain.Models;
using Smooth.Shop.Infrastructure.Data;

namespace Smooth.Shop.Infrastructure.Repos;

public class NewMediumRepository : INewMediumRepository
{
    private readonly SmoothWebDbContext _context;

    public NewMediumRepository(SmoothWebDbContext context)
    {
        _context = context;
    }

    public async Task<NewMedium?> GetByIdAsync(long id)
    {
        return await _context.NewMedia.FindAsync(id) ?? null;
    }

    public async Task<IEnumerable<NewMedium>> GetAllAsync()
    {
        return await _context.NewMedia.ToListAsync();
    }

    public async Task AddAsync(NewMedium newMedium)
    {
        _context.NewMedia.Add(newMedium);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _context.NewMedia.FindAsync(id);
        if (entity != null)
        {
            _context.NewMedia.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}