using ESPService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ESPService.Data;

public class AppDbContext : DbContext
{
    public DbSet<DiscountCode> Codes { get; set; }

    public AppDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
