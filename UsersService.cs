using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using UsersService.Models;

namespace Users.Register
{
    public static class UsersService
    {
        [FunctionName("AddUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Console.WriteLine(requestBody);
            var newUser = JsonConvert.DeserializeObject<User>(requestBody);
            if (newUser == null)
            {
                throw new Exception("test exception");
            }
            User user = new User(newUser);
            if (user == null)
            {
                throw new Exception("test exception");
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("UsersTable");
            table.CreateIfNotExistsAsync().Wait();
            table.ExecuteAsync(TableOperation.Insert(user)).Wait();

            string responseMessage = "success";

            return new OkObjectResult(responseMessage);
        }
    }
}
