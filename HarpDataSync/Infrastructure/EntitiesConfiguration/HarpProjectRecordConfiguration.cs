using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OldIrasSyncProjectData.Application.DTO;

namespace HarpDataSync.Infrastructure.EntitiesConfiguration;

public class HarpProjectRecordConfiguration : IEntityTypeConfiguration<HarpProjectRecord>
{
    public void Configure(EntityTypeBuilder<HarpProjectRecord> builder)
    {
        builder.HasKey(p => p.Id);

        builder
            .Property(p => p.Id)
            .HasColumnType("varchar(150)")
            .HasDefaultValueSql("CAST(NEWID() AS VARCHAR(150))");

        builder
            .HasIndex(x => x.IrasId)
            .IsUnique();
    }
}