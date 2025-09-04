namespace OldIrasSyncProjectData.Application.Contracts.Services
{
    public interface IHarpDataSyncService
    {
        /// <summary>
        /// Function that gets Project data from Old Iras system and updates our Validation Iras database.
        /// </summary>
        /// <returns>Return true if sync succeeded.</returns>
        Task<bool> SyncIrasProjectData();
    }
}