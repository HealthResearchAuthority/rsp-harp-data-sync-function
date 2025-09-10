using OldIrasSyncProjectData.Application.DTO;

namespace OldIrasSyncProjectData.Application.Contracts.Repositories
{
    public interface IHarpProjectDataRepository
    {
        Task UpdateProjectRecords(IEnumerable<HarpProjectRecord> oldIrasProjectRecords);
    }
}