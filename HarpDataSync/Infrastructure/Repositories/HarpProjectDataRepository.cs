using System.Data;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OldIrasSyncProjectData.Application.Contracts.Repositories;
using OldIrasSyncProjectData.Application.DTO;

namespace HarpDataSync.Infrastructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class HarpProjectDataRepository : IHarpProjectDataRepository
    {
        private readonly HarpProjectDataDbContext _context;
        private readonly ILogger<HarpProjectDataRepository> _logger;

        public HarpProjectDataRepository(HarpProjectDataDbContext context, ILogger<HarpProjectDataRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task UpdateProjectRecords(IEnumerable<HarpProjectRecord> oldIrasProjectRecords)
        {
            var now = DateTime.UtcNow;
            var irasIds = oldIrasProjectRecords.Select(r => r.IrasId).ToList();

            var existingRecords = await _context.HarpProjectRecords
                .Where(p => irasIds.Contains(p.IrasId))
                .ToDictionaryAsync(p => p.IrasId);

            var inputCount = oldIrasProjectRecords?.Count() ?? 0;
            var existingCount = existingRecords?.Count ?? 0;

            _logger.LogInformation("HARP sync: received {InputCount} records; {ExistingCount} match existing rows.",
                inputCount, existingCount);

            int updated = 0, inserted = 0;

            foreach (var source in oldIrasProjectRecords)
            {
                if (existingRecords.TryGetValue(source.IrasId, out var existing))
                {
                    // UPDATE
                    if (existing.LastSyncDate < source.DateRegistered)
                    {
                        existing.DateRegistered = source.DateRegistered;
                        existing.RecID = source.RecID;
                        existing.RecName = source.RecName;
                        existing.ShortStudyTitle = source.ShortStudyTitle;
                        existing.StudyDecision = source.StudyDecision;
                        existing.FullResearchTitle = source.FullResearchTitle;
                        existing.LastSyncDate = now;
                        updated++;
                    }
                }
                else
                {
                    // INSERT
                    var newRecord = new HarpProjectRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        IrasId = source.IrasId,
                        DateRegistered = source.DateRegistered,
                        RecID = source.RecID,
                        RecName = source.RecName,
                        ShortStudyTitle = source.ShortStudyTitle,
                        StudyDecision = source.StudyDecision,
                        FullResearchTitle = source.FullResearchTitle,
                        LastSyncDate = now
                    };
                    inserted++;
                    await _context.HarpProjectRecords.AddAsync(newRecord);
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("HARP sync completed: inserted={Inserted}, updated={Updated}, totalProcessed={Total}.",
                inserted, updated, inserted + updated);
        }
    }
}