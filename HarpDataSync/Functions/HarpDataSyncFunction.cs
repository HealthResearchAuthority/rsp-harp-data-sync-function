using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using OldIrasSyncProjectData.Application.Contracts.Services;

namespace OldIrasSyncProjectData.Functions
{
    public class HarpDataSyncFunction
    {
        private readonly ILogger<HarpDataSyncFunction> _logger;
        private readonly IHarpDataSyncService _service;

        public HarpDataSyncFunction(ILogger<HarpDataSyncFunction> logger, IHarpDataSyncService service)
        {
            _logger = logger;
            _service = service;
        }

        [Function("func-harp-data-sync")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            var syncSucceeded = await _service.SyncIrasProjectData();

            if (syncSucceeded)
            {
                _logger.LogInformation("Iras Projects Data Sync Succeeded");
                return req.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                _logger.LogWarning("Iras Projects Data Sync Failed");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}