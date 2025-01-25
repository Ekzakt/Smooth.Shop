using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Smooth.Shop.Data;

public class IdentityDbContext : DbContext, IDataProtectionKeyContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public required DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
}
