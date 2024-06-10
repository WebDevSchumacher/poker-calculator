using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace manager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculateController : ControllerBase
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Post(Game request)
        {
            HttpClient client = new HttpClient();
            string url = "http://node/Calculate";
            request.id = "newGame"; // TODO: fill with id from DB.insert()
            request.type = "holdem"; // TODO: fill in frontend
            string json = JsonSerializer.Serialize(request);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            
            Console.WriteLine("req: ");
            Console.WriteLine(json);
            
            var response = client.PostAsync(url, content);
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }
    }
}
