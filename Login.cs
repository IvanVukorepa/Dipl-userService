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
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using UsersService;
//using System.IdentityModel.Tokens.Jwt;

namespace Company.Function
{
    public static class Login
    {
        [FunctionName("Login")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

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

            string filterUsername = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, user.username);
            string filterPassword = TableQuery.GenerateFilterCondition("Password", QueryComparisons.Equal, user.Password);
            string filter = TableQuery.CombineFilters(filterUsername, TableOperators.And, filterPassword);

            var query = new TableQuery<User>().Where(filter);

            var res = await table.ExecuteQuerySegmentedAsync(query, null);
            if (res.Results.Count == 0)
                throw new Exception("User with credentials provided not found");
            string token = Auth_JWT.generateJWT(user);

            return new OkObjectResult(token);
        }
    }
}
