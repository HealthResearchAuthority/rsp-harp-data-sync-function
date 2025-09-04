using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
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

        var contextMock = new Mock<FunctionContext>();
        var request = new FakeHttpRequestData(contextMock.Object, new Uri("http://localhost/api"));

        // Act
        var response = await function.Run(request);

        // Assert
        var fakeResponse = Assert.IsType<FakeHttpResponseData>(response);
        Assert.Equal(HttpStatusCode.OK, fakeResponse.StatusCode);

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

        var contextMock = new Mock<FunctionContext>();
        var request = new FakeHttpRequestData(contextMock.Object, new Uri("http://localhost/api"));

        // Act
        var response = await function.Run(request);

        // Assert
        var fakeResponse = Assert.IsType<FakeHttpResponseData>(response);
        Assert.Equal(HttpStatusCode.InternalServerError, fakeResponse.StatusCode);

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

public class FakeHttpResponseData : HttpResponseData
{
    private readonly MemoryStream _bodyStream = new MemoryStream();

    public FakeHttpResponseData(FunctionContext req, HttpStatusCode statusCode) : base(req)
    {
        StatusCode = statusCode;
        Headers = new HttpHeadersCollection();
        Body = _bodyStream;
    }

    public override HttpStatusCode StatusCode { get; set; }
    public override HttpHeadersCollection Headers { get; set; }
    public override Stream Body { get; set; }
    public override HttpCookies Cookies => null;

    public async Task WriteStringAsync(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        await Body.WriteAsync(bytes, 0, bytes.Length);
        Body.Position = 0;
    }

    public string GetBodyAsString()
    {
        Body.Position = 0;
        return new StreamReader(Body).ReadToEnd();
    }
}

public class FakeHttpRequestData : HttpRequestData
{
    public FakeHttpRequestData(FunctionContext context, Uri uri, HttpStatusCode statusCode = HttpStatusCode.OK) : base(context)
    {
        Url = uri;
        Headers = new HttpHeadersCollection();
        Body = new MemoryStream();
        StatusCode = statusCode;
    }

    public override Stream Body { get; }
    public override HttpHeadersCollection Headers { get; }
    public override Uri Url { get; }

    public override string Method => "GET";

    public HttpStatusCode StatusCode { get; set; }

    public override IEnumerable<ClaimsIdentity> Identities => Array.Empty<ClaimsIdentity>();

    public override IReadOnlyCollection<IHttpCookie> Cookies => Array.Empty<IHttpCookie>();

    public override HttpResponseData CreateResponse()
    {
        return new FakeHttpResponseData(FunctionContext, StatusCode);
    }
}