namespace OldIrasSyncProjectData.Application.DTO
{
    public class HarpProjectRecord
    {
        public string Id { get; set; } = null!;
        public int IrasId { get; set; }
        public int? RecID { get; set; }
        public string? RecName { get; set; }
        public string? ShortProjectTitle { get; set; }
        public string? StudyDecision { get; set; }
        public DateTime DateRegistered { get; set; }
        public string? FullProjectTitle { get; set; }
        public DateTime LastSyncDate { get; set; }
    }
}