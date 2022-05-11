
using Azure.Core;
using Azure.Identity;
using CommandLine;
using DbUp;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SynapseSQLMigrator;
using System.Reflection;


Console.WriteLine("Starting Migration Helper");

CommandLineOptions? options = null;

Parser.Default.ParseArguments<CommandLineOptions>(args)
                   .WithParsed<CommandLineOptions>(o =>
                   {
                       options = o;
                   }).WithNotParsed(x=>
                   {
                       Console.WriteLine("Missing parameters");
                   });

if (options == null)
    return -1;

ClientSecretCredential credential = new ClientSecretCredential(
                    options.TenantId, options.ServicePrincipalId, options.ServicePrincipalPassword, new TokenCredentialOptions());

var journalOptions = new AzureTableStorageJournalOptions()
{
    Container = options.AzureStorageContainer,
    JournalPath = options.AzureStoragePath,
    AzureStorageAccountName = options.AzureStorageAccountName,
    ClientCredential = credential,
};


string sqlConnectionString = string.Format("Data Source=tcp:{0},1433;Initial Catalog={1};Persist Security Info=False;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False", options.SqlServerName, options.SqlDatabase);

var scriptsPath = options.ScriptsPath ?? Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.FullName, "Scripts");

var upgrader =
       DeployChanges.To
           .SqlDatabase(new SQLServerlessConnectionManager(sqlConnectionString, new ClientCredential(options.ServicePrincipalId, options.ServicePrincipalPassword), options.TenantId))
           .WithScriptsFromFileSystem(scriptsPath)
           .JournalTo(new AzureTableStorageJournal(journalOptions))
           .LogToConsole()
           .Build();


var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.ResetColor();
#if DEBUG
    Console.ReadLine();
#endif
    return -1;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Success!");
Console.ResetColor();
return 0;
