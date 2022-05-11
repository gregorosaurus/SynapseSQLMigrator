
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CsvHelper;
using CsvHelper.Configuration;
using DbUp.Engine;
using System.Data;
using System.Globalization;

namespace SynapseSQLMigrator
{

    public class AzureTableStorageJournalOptions
    {
        public string AzureStorageAccountName { get; set; }
        public TokenCredential ClientCredential { get; set; }
        public string Container { get; set; }
        public string JournalPath { get; set; }
    }

    class AzureTableStorageJournal : IJournal
    {
        private BlobClient _journalBlobClient;
        private CsvConfiguration _csvConfig;
        private class JournalRecord
        {
            public string? Script { get; set; }
            public DateTime? ExecutedAt { get; set; }
        }

        public AzureTableStorageJournal(AzureTableStorageJournalOptions options)
        {
            string blobUri = $"https://{options.AzureStorageAccountName}.blob.core.windows.net";

            var serviceClient = new BlobServiceClient(new Uri(blobUri), options.ClientCredential);
            var containerClient = serviceClient.GetBlobContainerClient(options.Container);
            _journalBlobClient = containerClient.GetBlobClient(options.JournalPath);

            _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };
        }

        public void EnsureTableExistsAndIsLatestVersion(Func<IDbCommand> dbCommandFactory)
        {
            //not required, I think
        }

        public string[] GetExecutedScripts()
        {
            return CurrentJournaledScripts().Select(x => x.Script!).ToArray();
        }

        private IEnumerable<JournalRecord> CurrentJournaledScripts()
        {
            if (!_journalBlobClient.Exists())
            {
                return new JournalRecord[0];
            }

            using (Stream blobStream = _journalBlobClient.OpenRead())
            using (StreamReader sr = new StreamReader(blobStream))
            using (CsvReader csvReader = new CsvReader(sr, _csvConfig))
            {
                return csvReader.GetRecords<JournalRecord>().ToArray();
            }
        }

        public void StoreExecutedScript(SqlScript script, Func<IDbCommand> dbCommandFactory)
        {
            var currentRecords = CurrentJournaledScripts().ToList();

            currentRecords.Add(new JournalRecord()
            {
                ExecutedAt = DateTime.UtcNow,
                Script = script.Name
            });

            using (Stream blobStream = _journalBlobClient.OpenWrite(overwrite: true, new BlobOpenWriteOptions()))
            using (StreamWriter sw = new StreamWriter(blobStream))
            {
                using (CsvWriter csvWriter = new CsvWriter(sw, _csvConfig))
                {
                    csvWriter.WriteRecords(currentRecords);
                }
            }
        }
    }

}