using System.Data;
using Microsoft.EntityFrameworkCore;
using OldIrasSyncProjectData.Application.Contracts.Repositories;
using OldIrasSyncProjectData.Application.DTO;

namespace HarpDataSync.Infrastructure.Repositories
{
    public class HarpProjectDataRepository : IHarpProjectDataRepository
    {
        private readonly HarpProjectDataDbContext _context;

        public HarpProjectDataRepository(HarpProjectDataDbContext context)
        {
            _context = context;
        }

        public async Task UpdateProjectRecords(IEnumerable<HarpProjectRecord> oldIrasProjectRecords)
        {
            var now = DateTime.UtcNow;
            var irasIds = oldIrasProjectRecords.Select(r => r.IrasId).ToList();

            var existingRecords = await _context.HarpProjectRecords
                .Where(p => irasIds.Contains(p.IrasId))
                .ToDictionaryAsync(p => p.IrasId);

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
                    }
                }
                else
                {
                    // INSERT
                    var newRecord = new HarpProjectRecord
                    {
                        Id = "", // Temp Id will be updated by database.
                        IrasId = source.IrasId,
                        DateRegistered = source.DateRegistered,
                        RecID = source.RecID,
                        RecName = source.RecName,
                        ShortStudyTitle = source.ShortStudyTitle,
                        StudyDecision = source.StudyDecision,
                        FullResearchTitle = source.FullResearchTitle,
                        LastSyncDate = now
                    };

                    await _context.HarpProjectRecords.AddAsync(newRecord);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}