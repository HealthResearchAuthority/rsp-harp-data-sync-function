using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
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
        public async Task<IActionResult> Run([TimerTrigger("%SyncTimerSchedule%", RunOnStartup = false, UseMonitor = true)] TimerInfo myTimer)
        {
            var syncSucceeded = await _service.SyncIrasProjectData();

            if (syncSucceeded)
            {
                _logger.LogInformation("Iras Projects Data Sync Succeeded");
                return new OkObjectResult("Iras Projects Data Sync Succeeded");
            }
            else
            {
                _logger.LogWarning("Iras Projects Data Sync Failed");
                return new ObjectResult(new
                {
                    error = "INTERNAL_SERVER_ERROR"
                })
                {
                    StatusCode = 500
                };
            }
        }
    }
}