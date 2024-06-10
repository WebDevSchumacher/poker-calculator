using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using contracts;

namespace node.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculateController : ControllerBase
    {
        [ImportMany]
        IEnumerable<Lazy<IGameLogic, IGameLogicName>> logics;
        private CompositionContainer container;
        public CalculateController()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog("./GameLogic"));
            container = new CompositionContainer(catalog);
            try
            {
                container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }
        [HttpPost]
        public HttpResponseMessage Post(Game request)
        {
            string url = "http://manager/";
            double winPercentage = 12.0;
            foreach (Lazy<IGameLogic, IGameLogicName> logic in logics)
            {
                if (logic.Metadata.Name == request.type)
                {
                    winPercentage = logic.Value.CalculateWinProbability(request);    
                }
            }
            request.winpercentage = winPercentage;
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }
        
        [HttpGet]
        public string Get()
        {
            string res = "";
            return res;
        }
    }
}
