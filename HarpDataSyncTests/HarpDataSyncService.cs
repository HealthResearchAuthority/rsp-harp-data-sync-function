using Microsoft.Extensions.Logging;
using Moq;
using OldIrasSyncProjectData.Application.Contracts.Repositories;
using OldIrasSyncProjectData.Application.DTO;
using OldIrasSyncProjectData.Services;

namespace HarpDataSyncTests;

public class HarpDataSyncServiceTests
{
    private readonly Mock<IOldIrasProjectRepository> _oldRepoMock;
    private readonly Mock<IHarpProjectDataRepository> _harpRepoMock;
    private readonly Mock<ILogger<HarpDataSyncService>> _loggerMock;
    private readonly HarpDataSyncService _service;

    public HarpDataSyncServiceTests()
    {
        _oldRepoMock = new Mock<IOldIrasProjectRepository>();
        _harpRepoMock = new Mock<IHarpProjectDataRepository>();
        _loggerMock = new Mock<ILogger<HarpDataSyncService>>();
        _service = new HarpDataSyncService(_oldRepoMock.Object, _harpRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task SyncIrasProjectData_ReturnsFalse_WhenNoRecords()
    {
        // Arrange
        _oldRepoMock
            .Setup(r => r.GetProjectRecords())
            .ReturnsAsync(Enumerable.Empty<HarpProjectRecord>());

        // Act
        var result = await _service.SyncIrasProjectData();

        // Assert
        Assert.False(result);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Cannot get records from old iras database.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SyncIrasProjectData_ReturnsTrue_WhenRecordsExistAndUpdateSucceeds()
    {
        // Arrange
        var records = new List<HarpProjectRecord>
        {
            new HarpProjectRecord { IrasId = 1, RecName = "Test" }
        };

        _oldRepoMock
            .Setup(r => r.GetProjectRecords())
            .ReturnsAsync(records);

        _harpRepoMock
            .Setup(r => r.UpdateProjectRecords(records))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.SyncIrasProjectData();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task SyncIrasProjectData_ReturnsFalse_WhenUpdateThrowsException()
    {
        // Arrange
        var records = new List<HarpProjectRecord>
        {
            new HarpProjectRecord { IrasId = 1, RecName = "Test" }
        };

        _oldRepoMock
            .Setup(r => r.GetProjectRecords())
            .ReturnsAsync(records);

        _harpRepoMock
            .Setup(r => r.UpdateProjectRecords(records))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.SyncIrasProjectData();

        // Assert
        Assert.False(result);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error during sync for IRAS ID")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}