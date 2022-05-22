using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using UsersService.Models;
using System.Collections.Generic;

namespace Company.Function
{
    public static class GetAllFiltered
    {
        [FunctionName("GetAllFiltered")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string receivedfilter = req.Query["filter"];

            if (string.IsNullOrEmpty(receivedfilter))
                return await Task.FromException<IActionResult>(new Exception("test exception")); 

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("UsersTable");
            

            string filter = createFilter(receivedfilter);
            var query = new TableQuery<User>().Where(filter);

            var res = await table.ExecuteQuerySegmentedAsync(query, null);

            List<string> users = new List<string>();
            foreach(var u in res.Results){
                users.Add(u.username);
            }

            return new OkObjectResult(users);
        }

        public static string createFilter(string receivedfilter){
            string filter = "";

            int paternLen = receivedfilter.Length;
            string greaterThanPattern = receivedfilter;
            string lowerThanPattern = receivedfilter.Substring(0, paternLen-1);
            char c = (char)(Convert.ToInt16(receivedfilter[paternLen-1]) + 1);
            lowerThanPattern = lowerThanPattern + c;

            string filter1 = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, greaterThanPattern);
            string filter2 = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, lowerThanPattern);
            filter = TableQuery.CombineFilters(filter1, TableOperators.And, filter2);

            return filter;

        }
    }
}
