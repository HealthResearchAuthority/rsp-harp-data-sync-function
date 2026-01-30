using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.SqlClient;
using OldIrasSyncProjectData.Application.Contracts.Repositories;
using OldIrasSyncProjectData.Application.DTO;

namespace HarpDataSync.Infrastructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class OldIrasProjectRepository : IOldIrasProjectRepository
    {
        private readonly string _connectionString;

        public OldIrasProjectRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<HarpProjectRecord>> GetProjectRecords()
        {
            var records = new List<HarpProjectRecord>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sql = @"
                SELECT
                   app.RefIRASProjectID AS IRAS_ID,
                   app.CommitteeID AS Rec_ID,
                   rec.Name AS Rec_Name,
                   app.ApplicationTitle AS Short_Study_Title,
                   app.ApplicationDecisionText AS Study_Decision,
                   app.DateRegistered,
                   cr1.field3 AS [Full Research Title]
                FROM APP_Application AS app
                    LEFT JOIN APP_Committee AS rec ON app.CommitteeID = rec.CommitteeID
                    LEFT JOIN APP_StudyType AS studytype ON app.StudyTypeID = studytype.StudyTypeID
                    LEFT JOIN APP_ApplicationToProject AS atp ON atp.ApplicationID = app.ApplicationID AND atp.IsInUse = 1
                    LEFT JOIN CR_ProjectData_LongText_1 AS cr1 ON cr1.ProjectID = atp.ProjectID
                WHERE app.ParentApplicationID IS NULL
                    AND app.ApplicationDecisionText IN ('Favourable Opinion', 'Further Information Favourable Opinion')
                    AND app.PostApprovalStateText IN ('Halted Temporary','Not Started','Started','Notification to Suspend')
                    AND app.CommitteeID IN (316, 313, 315, 314)
                    AND studytype.StudyType NOT IN ('Research Tissue Bank','Research Database')
                    AND app.StudyTypeID IN (6)
                    AND app.RefIRASProjectID is not null and app.RefIRASProjectID != ''";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.CommandTimeout = 300;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (!int.TryParse(reader["IRAS_ID"]?.ToString(), out var irasId))
                                continue;

                            int? recId = int.TryParse(reader["Rec_ID"]?.ToString(), out var parsedRecId) ? parsedRecId : null;
                            DateTime dateRegistered = DateTime.TryParse(reader["DateRegistered"]?.ToString(), out var parsedDate)
                                ? parsedDate
                                : DateTime.MinValue;

                            records.Add(new HarpProjectRecord
                            {
                                IrasId = irasId,
                                RecID = recId,
                                RecName = reader["Rec_Name"]?.ToString(),
                                ShortStudyTitle = reader["Short_Study_Title"]?.ToString(),
                                StudyDecision = reader["Study_Decision"]?.ToString(),
                                DateRegistered = dateRegistered,
                                FullResearchTitle = reader["Full Research Title"]?.ToString()
                            });
                        }
                    }
                }
            }
            return records;
        }
    }
}