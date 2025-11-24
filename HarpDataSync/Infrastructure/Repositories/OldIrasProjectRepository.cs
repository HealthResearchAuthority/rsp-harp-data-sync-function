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
        private readonly string _getProjectRecordsQuery;

        public OldIrasProjectRepository(string connectionString, string getProjectRecordsQuery)
        {
            _connectionString = connectionString;
            _getProjectRecordsQuery = getProjectRecordsQuery;
        }

        public async Task<IEnumerable<HarpProjectRecord>> GetProjectRecords()
        {
            var records = new List<HarpProjectRecord>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(_getProjectRecordsQuery, connection))
                {
                    command.CommandTimeout = 300;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (!int.TryParse(reader["IRASId"]?.ToString(), out var irasId))
                                continue;

                            int? recId = int.TryParse(reader["RecId"]?.ToString(), out var parsedRecId) ? parsedRecId : null;
                            DateTime dateRegistered = DateTime.TryParse(reader["Date Registered"]?.ToString(), out var parsedDate)
                                ? parsedDate
                                : DateTime.MinValue;

                            records.Add(new HarpProjectRecord
                            {
                                IrasId = irasId,
                                RecID = recId,
                                RecName = reader["Rec Name"]?.ToString(),
                                ShortProjectTitle = reader["Short Project Title"]?.ToString(),
                                StudyDecision = reader["Study Decision"]?.ToString(),
                                DateRegistered = dateRegistered,
                                FullProjectTitle = reader["Full Project Title"]?.ToString()
                            });
                        }
                    }
                }
            }
            return records;
        }
    }
}