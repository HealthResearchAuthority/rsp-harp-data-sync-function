using HarpDataSync.Infrastructure.EntitiesConfiguration;
using Microsoft.EntityFrameworkCore;
using OldIrasSyncProjectData.Application.DTO;

namespace HarpDataSync.Infrastructure;

public class HarpProjectDataDbContext(DbContextOptions<HarpProjectDataDbContext> options) : DbContext(options)
{
    public DbSet<HarpProjectRecord> HarpProjectRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new HarpProjectRecordConfiguration());
    }
}