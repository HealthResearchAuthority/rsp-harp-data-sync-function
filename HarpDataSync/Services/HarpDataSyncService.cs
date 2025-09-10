using Microsoft.Extensions.Logging;
using OldIrasSyncProjectData.Application.Contracts.Repositories;
using OldIrasSyncProjectData.Application.Contracts.Services;
using OldIrasSyncProjectData.Application.DTO;

namespace OldIrasSyncProjectData.Services;

public class HarpDataSyncService : IHarpDataSyncService
{
    private readonly IOldIrasProjectRepository _oldIrasProjectRepository;
    private readonly IHarpProjectDataRepository _harpProjectRepository;
    private readonly ILogger<HarpDataSyncService> _logger;

    public HarpDataSyncService(
            IOldIrasProjectRepository oldIrasProjectRepository,
            IHarpProjectDataRepository harpProjectRepository,
            ILogger<HarpDataSyncService> logger)
    {
        _oldIrasProjectRepository = oldIrasProjectRepository;
        _harpProjectRepository = harpProjectRepository;
        _logger = logger;
    }

    public async Task<bool> SyncIrasProjectData()
    {
        IEnumerable<HarpProjectRecord> sourceRecords = await _oldIrasProjectRepository.GetProjectRecords();

        if (!sourceRecords.Any())
        {
            _logger.LogError("Cannot get records from old iras database.");
            return false;
        }

        try
        {
            await _harpProjectRepository.UpdateProjectRecords(sourceRecords);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sync for IRAS ID");
            return false;
        }
    }
}