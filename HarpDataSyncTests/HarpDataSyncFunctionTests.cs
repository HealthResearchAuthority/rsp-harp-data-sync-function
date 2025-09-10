using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;
using OldIrasSyncProjectData.Application.Contracts.Services;
using OldIrasSyncProjectData.Functions;

namespace HarpDataSyncTests;

public class HarpDataSyncFunctionTests
{
    [Fact]
    public async Task Run_ReturnsOk_WhenSyncSucceeds()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<HarpDataSyncFunction>>();
        var serviceMock = new Mock<IHarpDataSyncService>();
        serviceMock.Setup(s => s.SyncIrasProjectData()).ReturnsAsync(true);

        var function = new HarpDataSyncFunction(loggerMock.Object, serviceMock.Object);

        var timerInfo = new TimerInfo();

        // Act
        var result = await function.Run(timerInfo);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal("Iras Projects Data Sync Succeeded", okResult.Value);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Iras Projects Data Sync Succeeded")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Run_ReturnsInternalServerError_WhenSyncFails()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<HarpDataSyncFunction>>();
        var serviceMock = new Mock<IHarpDataSyncService>();
        serviceMock.Setup(s => s.SyncIrasProjectData()).ReturnsAsync(false);

        var function = new HarpDataSyncFunction(loggerMock.Object, serviceMock.Object);

        var timerInfo = new TimerInfo();

        // Act
        var result = await function.Run(timerInfo);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Iras Projects Data Sync Failed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}