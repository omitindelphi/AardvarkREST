using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AardvarkREST.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using AardvarkREST.Filters;

namespace AardvarkREST.Controllers
{
    /// <summary>
    /// Class to manipulate by creation and destruction of Workflow Charts as container for Tasks and items. 
    /// Chart defined by ChartName. ChartDescripption can be omitted. ChartId passed to POST, PUT requests can be omitted 
    /// DELETE request does require int ChartId; deletion of the chart removes all its Tasks and Items
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("api/WFCharts")]
    //[WFChartAPIExceptionFilter]
    public class WFChartsController : Controller
    {
       // private readonly WFContext _context;
        private readonly IWFChartRepository _repository;

        /// <summary>
        /// Class to manipulate by creation and destruction of Workflow Charts as container for Tasks and items. 
        /// Chart defined by ChartName. ChartDescripption can be omitted. ChartId passed to POST, PUT, DELETE requests is omitted.
        /// </summary>
        public WFChartsController( IWFChartRepository repository)
        {
           // _context = context;
            _repository = repository;
        }

        // GET: api/WFCharts/ChartName
        /// <summary>
        /// Retrieves ChartId and ChartName for all Workflow Charts. No parameters expected
        /// </summary>
        /// <returns>Collection of {ChartId, ChartName, ChartDescription} objects</returns>
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<WFChart>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IEnumerable<IErrorObject>))]
        public IEnumerable<WFChart> GetChart()
        {
            return _repository.FindAll().Result;
        }

        // GET: api/WFCharts/Abracadabra
        /// <summary>
        /// Retrieves ChartId,ChartName and ChartDescription for specific Workflow Chart
        /// </summary>
        /// <param name="ChartName">The name of the item to be retrieved</param>
        /// <returns> {ChartId, ChartName, ChartDescription} object </returns>
        [HttpGet("{ChartName}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(WFChart))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IEnumerable<IErrorObject>))]
        public async Task<IActionResult> GetWFChart([FromRoute] string ChartName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            WFChart wFChart = await _repository.Get(ChartName);

            if (wFChart == null)
            {
                return NotFound(string.Format("Not found Chart Name '{0}'", ChartName));
            }

            return Ok(wFChart);
        }

        /// POST: api/WFCharts
        /// PUT:  api/WFCharts
        /// <summary>
        /// Creates or alters Workflow Chart from body of request; expects body of single  {ChartId, ChartName, ChartDescription} object
        /// </summary>
        /// <returns>{ChartId, ChartName, ChartDescription} object with populated ChartId</returns>
        [HttpPost]
        [HttpPut]
        public async Task<IActionResult> PostWFChart([FromBody] WFChart wfChart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return await ProcessInsertUpdate(wfChart);
        }

        [HttpPost("{ChartName}")]
        [HttpPut("{ChartName}")]
        [HttpPost("{ChartName}/{ChartDescription}")]
        [HttpPut("{ChartName}/{ChartDescription}")]
        public async Task<IActionResult> PostWFChartRoute([FromRoute] WFChart wfChart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return await ProcessInsertUpdate(wfChart);
        }

        private async Task<IActionResult> ProcessInsertUpdate(WFChart wfChart)
        {
            try
            {
                WFChart resultChart = await _repository.Save(wfChart);
                var ret = Ok(resultChart);
                return ret;
            }
            catch (Exception e)
            {
                var ret = BadRequest(string.Format("Problem on new chart creation: \"{0}\" " + e.Message, wfChart.ChartName));
                ret.StatusCode = (int)HttpStatusCode.Conflict;
                return ret;
            }
        }

        [HttpDelete("DeleteById/{id}")]
        public async Task<IActionResult> DeleteWFChartById([FromRoute] int id)
        {
            try
            {
                await _repository.DeleteById(id);
                return Ok();
            }
            catch (SQLWFException exx)
            {
                var ret = BadRequest(exx.Message);
                ret.StatusCode = (int)HttpStatusCode.NotFound;
                return ret;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}