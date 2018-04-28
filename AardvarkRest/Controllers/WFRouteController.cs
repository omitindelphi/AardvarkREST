using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AardvarkREST.Models;

namespace AardvarkREST.Controllers
{
    [Produces("application/json")]
    [Route("api/WFRoute")]
    public class WFRouteController : Controller
    {

        private readonly IWFRouteRepository _routeRepository;

        /// <summary>
        /// Class to manipulate by creation and destruction of Workflow Routes as links between Tasks in Container
        /// Route has Code as short string, default "Ok". Connects from TaskFrom to TaskTo.
        /// </summary>
        public WFRouteController(IWFRouteRepository existingRouteRepository)
        {
            _routeRepository = existingRouteRepository;
        }

        // GET: api/WFRoute/ChartName
        [HttpGet("{ChartName}", Name = "GetAllRoutes")]
        public IEnumerable<WFRoute> GetAllRoutes(string ChartName)
        {
            return _routeRepository.FindAll(ChartName).Result;
        }

        [HttpGet("{ChartName}/TaskFrom/{TaskFrom}/TaskTo/{TaskTo}", Name = "GetSingleRoute")]
        public async Task<IActionResult> GetSingleRoute(string ChartName, string TaskFrom, string TaskTo)
        {
            var aRet = await _routeRepository.Get(ChartName, TaskFrom, TaskTo);
            WFRoute result = aRet;
            return Ok(result);
        }

        // POST: api/WFRoute
        [HttpPost("{ChartName}/TaskFrom/{TaskFrom}/TaskTo/{TaskTo}/RouteCode/{RouteCode}", Name = "PostFullRoute")]
        [HttpPut("{ChartName}/TaskFrom/{TaskFrom}/TaskTo/{TaskTo}/RouteCode/{RouteCode}", Name = "PostFullRoute")]
        public async Task<IActionResult> PostFullRoute(string ChartName, string TaskFrom, string TaskTo, string RouteCode)
        {
            WFRoute route = new WFRoute { RouteId = 0, ChartId = 0, ChartName = ChartName, TaskFrom = TaskFrom, TaskTo = TaskTo, RouteCode = RouteCode };
            WFRoute resp = await _routeRepository.Save(route);
            return Ok(resp);
        }

        [HttpPost("{ChartName}/TaskFrom/{TaskFrom}/TaskTo/{TaskTo}", Name = "PostShortRoute")]
        public async Task<IActionResult> PostShortRoute(string ChartName, string TaskFrom, string TaskTo, string RouteCode)
        {
            WFRoute route = new WFRoute { RouteId = 0, ChartId = 0, ChartName = ChartName, TaskFrom = TaskFrom, TaskTo = TaskTo, RouteCode = WFRoute.DefaultRouteCode };
            WFRoute resp = await _routeRepository.Save(route);
            return Ok(resp);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{ChartName}/TaskFrom/{TaskFrom}/TaskTo/{TaskTo}", Name = "DeleteRoute")]
        public async Task<IActionResult> DeleteRoute(string ChartName, string TaskFrom, string TaskTo)
        {
            await _routeRepository.DeleteByNames(ChartName, TaskFrom, TaskTo);
            return Ok();
        }
        
    }
}
