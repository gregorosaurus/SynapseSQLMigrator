using Azure.Core;
using DbUp.Engine.Transactions;
using DbUp.Support;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseSQLMigrator
{
    public class SQLServerlessConnectionManager : DatabaseConnectionManager
    {
        public SQLServerlessConnectionManager(string connectionString, ClientCredential clientCredential, string tenantId,  string resourceId = "https://database.windows.net/", string aadInstance = "https://login.microsoftonline.com/")
           : base(new DelegateConnectionFactory((log, dbManager) =>
           {
               AuthenticationContext authContext = new AuthenticationContext(aadInstance + "/" + tenantId);
               AuthenticationResult authResult = authContext.AcquireTokenAsync(resourceId, clientCredential).Result;
               var conn = new SqlConnection(connectionString)
               {
                    AccessToken = authResult.AccessToken
               };

               if (dbManager.IsScriptOutputLogged)
                   conn.InfoMessage += (sender, e) => log.WriteInformation($"{{0}}", e.Message);

               return conn;
           }))
        { }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var commandSplitter = new SqlCommandSplitter();
            var scriptStatements = commandSplitter.SplitScriptIntoCommands(scriptContents);
            return scriptStatements;
        }
    }
}
