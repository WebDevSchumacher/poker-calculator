using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace manager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResultController : ControllerBase
    {
        [HttpGet]
        public Result Get(string id)
        {
            // get 
            Console.WriteLine(id);
            return new Result
            {
                aText = "something"
            };
        }
    }
}
