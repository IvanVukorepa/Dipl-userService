using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using UsersService.Models;

namespace Company.Function
{
    public static class CheckUsername
    {
        [FunctionName("CheckUsername")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string result = "false";
            string usernameToCheck = req.Query["username"];
            if (string.IsNullOrEmpty(usernameToCheck))
                return await Task.FromException<IActionResult>(new Exception("test exception")); 

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("UsersTable");

            string filter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, usernameToCheck);
            var query = new TableQuery<User>().Where(filter);

            var res = await table.ExecuteQuerySegmentedAsync(query, null);
            if (res.Results.Count == 0)
                result = "true";
            
            return new OkObjectResult(result);
        }
    }
}
