using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseSQLMigrator
{
    public class CommandLineOptions
    {
        [Option('s', "storage", HelpText = "The Azure storage account name", Required = true)]
        public string AzureStorageAccountName { get; set; }

        [Option('u', "spid", HelpText = "The service principal id to authenticate as", Required = true)]
        public string ServicePrincipalId { get; set; }

        [Option('p', "sppass", HelpText = "The service principal password", Required = true)]
        public string ServicePrincipalPassword { get; set; }

        [Option('t', "tenantid", HelpText = "The service principal's tenant ID", Required = true)]
        public string TenantId { get; set; }

        [Option('c', "container", HelpText = "The container where the journal will be stored", Required = true)]
        public string AzureStorageContainer { get; set; }

        [Option('p', "path", HelpText = "The path that the migration journal will be stored", Required = true)]
        public string AzureStoragePath { get; set; }

        [Option('q', "sqlserver", HelpText = "SQL servername of the sql serverless pool", Required = true)]
        public string SqlServerName { get; set; }
        
        [Option('d', "sqldb", HelpText = "SQL database to run the sql scripts against", Required = true)]
        public string SqlDatabase { get; set; }

        [Option('x', "scriptspath", Required = false, HelpText = "This is the path to access the scripts at.  By default, it will look in the executing directory for a folder called 'Scripts'.")]
        public string? ScriptsPath { get; set; }

    }
}
