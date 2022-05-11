# Synapse SQL Migrator

The Synapse SQL Migrator was created to help facilitate the migration and CI/CD process for SQL Serverless database tables and views.  

> Note: At the time of writing, SSDT does not support SQL Serverless.  This was created to bridge that gap. 

This tool uses the third party [DbUp library](https://github.com/DbUp/DbUp) to help run SQL migrations that are included as part of a SQL Serverless deployment in Synapse.   This tool uses Azure Blob Storage to keep track of run scripts via a CSV file.  Additionally, it uses Azure AD to authenticate against the SQL serverless database. Basically this tool is a superset of features on top of DbUp. 

It is expected that this be run as *part* of a CI/CD pipeline in either Azure DevOps or GitHub using GitHub Actions. The tool however can be used externally of those tools if required.  

## Authentication Specifics

The Synapse SQL Migrator uses a service principal to authenticate to both blob storage (where the migration history is kept), as well as to authenticate to the Synapse 
SQL serverless database. 

## Usage

The SSM (Synapse SQL Migrator) is used and executed via command line arguments.  Below lists the argument and the description of that argument.  All arguments are required unless specified as optional. 

| Argument | Description | 
|----------|-------------|
| --storage | This is the storage account name where the migration history is kept |
| --container | The container in the storage account where to save the migrations journal |
| --path | The relative path in the container of the storage account of where to save the migrations journal |
| --spid | The service principal client ID of the SP that will be used for authentication | 
| --sppass | The service principal secrete used to authenticate the SP | 
| --tenantid | The tenant ID that the service principal belongs to |
| --sqlserver | The SQL server name of the Synapse Serverless database, example `bestserver-ondemand.sql.azuresynapse.net` |
| --sqldb | The name of the SQL serverless database to run migrations against |
| --scriptspath | The local path, absolute or relative, of a directory containing script files | 

## CI/CD Pipelines

At a high level, the following process should be followed when building out a CI/CD process for SQL serverless database objects.
1. Clone the Synapse SQL Migrator repo. 
2. Build the solution .
3. Run the SynapseSqlMigrator.exe with the required arguments.
4. Clean up.
5. Continue with other CI/CD tasks/steps. 

