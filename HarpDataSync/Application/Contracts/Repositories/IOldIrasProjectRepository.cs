using OldIrasSyncProjectData.Application.DTO;

namespace OldIrasSyncProjectData.Application.Contracts.Repositories
{
    public interface IOldIrasProjectRepository
    {
        Task<IEnumerable<HarpProjectRecord>> GetProjectRecords();
    }
}