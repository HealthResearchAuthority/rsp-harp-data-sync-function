using HarpDataSync.Infrastructure;
using HarpDataSync.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using OldIrasSyncProjectData.Application.DTO;

namespace HarpDataSyncTests;

public class HarpProjectDataRepository_UpdateTests
{
    private readonly HarpProjectDataDbContext _context;
    private readonly HarpProjectDataRepository _repository;

    public HarpProjectDataRepository_UpdateTests()
    {
        var options = new DbContextOptionsBuilder<HarpProjectDataDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        _context = new HarpProjectDataDbContext(options);
        _repository = new HarpProjectDataRepository(_context);

        SeedInitialData();
    }

    private void SeedInitialData()
    {
        _context.HarpProjectRecords.Add(new HarpProjectRecord
        {
            Id = Guid.NewGuid().ToString("N"),
            IrasId = 1001,
            RecID = 1,
            RecName = "Initial",
            ShortStudyTitle = "Initial Title",
            StudyDecision = "Pending",
            DateRegistered = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            FullResearchTitle = "Initial Full Title",
            LastSyncDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        _context.SaveChanges();
    }

    [Fact]
    public async Task UpdateProjectRecords_UpdatesExistingRecord_WhenNewerDateRegistered()
    {
        // Arrange
        var updatedRecord = new HarpProjectRecord
        {
            IrasId = 1001,
            RecID = 2,
            RecName = "Updated",
            ShortStudyTitle = "Updated Title",
            StudyDecision = "Approved",
            DateRegistered = new DateTime(2023, 5, 1, 0, 0, 0, DateTimeKind.Utc),
            FullResearchTitle = "Updated Full Title",
            LastSyncDate = new DateTime(2023, 5, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        await _repository.UpdateProjectRecords([updatedRecord]);

        // Assert
        var result = await _context.HarpProjectRecords.FirstOrDefaultAsync(r => r.IrasId == 1001);
        Assert.NotNull(result);
        Assert.Equal("Updated", result!.RecName);
        Assert.Equal("Approved", result.StudyDecision);
        Assert.Equal("Updated Full Title", result.FullResearchTitle);
        Assert.True(result.LastSyncDate > new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public async Task UpdateProjectRecords_DoesNotUpdate_WhenDateRegisteredIsOlder()
    {
        // Arrange
        var olderRecord = new HarpProjectRecord
        {
            IrasId = 1001,
            RecID = 3,
            RecName = "ShouldNotUpdate",
            ShortStudyTitle = "Old Title",
            StudyDecision = "Rejected",
            DateRegistered = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            FullResearchTitle = "Old Full Title",
            LastSyncDate = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        await _repository.UpdateProjectRecords([olderRecord]);

        // Assert
        var result = await _context.HarpProjectRecords.FirstOrDefaultAsync(r => r.IrasId == 1001);
        Assert.NotNull(result);
        Assert.NotEqual("ShouldNotUpdate", result!.RecName);
    }

    [Fact]
    public async Task UpdateProjectRecords_InsertsNewRecord_WhenNotExists()
    {
        // Arrange
        var newRecord = new HarpProjectRecord
        {
            Id = Guid.NewGuid().ToString("N"),
            IrasId = 2002,
            RecID = 5,
            RecName = "New Record",
            ShortStudyTitle = "New Study",
            StudyDecision = "Approved",
            DateRegistered = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            FullResearchTitle = "New Full Title",
            LastSyncDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        await _repository.UpdateProjectRecords([newRecord]);

        // Assert
        var result = await _context.HarpProjectRecords.FirstOrDefaultAsync(r => r.IrasId == 2002);
        Assert.NotNull(result);
        Assert.Equal("New Record", result!.RecName);
        Assert.Equal("New Study", result.ShortStudyTitle);
    }
}