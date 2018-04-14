using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc;
using AardvarkREST.Models;
using System.Net;

namespace AardvarkREST.Controllers
{
    [Produces("application/json")]
    [Route("api/WFTask")]
    public class WFTaskController : Controller
    {
        private readonly IWFTaskRepository _taskRepository;

        /// <summary>
        /// Class to manipulate by creation and destruction of Workflow Charts as container for Tasks and items. 
        /// Chart defined by ChartName. ChartDescripption can be omitted. ChartId passed to POST, PUT, DELETE requests is omitted.
        /// </summary>
        public WFTaskController(IWFTaskRepository existingTaskRepository)
        {
            // _context = context;
            _taskRepository = existingTaskRepository;
        }

        // GET: api/WFTask
        [HttpGet]
        [Route("{ChartName}", Name = "GetAll")]
        public IEnumerable<WFTask> GetAll([FromRoute]string ChartName)
        {
            // return new string[] { "value1", "value2", ChartName };
            return _taskRepository.FindAll(ChartName).Result;
        }

        // GET: api/WFTask/5
        [HttpGet("{ChartName}/Task/{TaskName}", Name = "Get")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(WFChart))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IEnumerable<IErrorObject>))]
        public async Task<IActionResult> Get(string ChartName, string TaskName)
        {
            WFTask aWFTask = await _taskRepository.Get(ChartName, TaskName);

            if (aWFTask == null)
            {
                return NotFound(string.Format("Not found Chart Name '{0}' Task name '{1}'", ChartName, TaskName));
            }
            return Ok(aWFTask);
        }

        // POST: api/WFTask
        [HttpPost("{ChartName}/Task/{TaskName}", Name = "Post")]
        public async Task<IActionResult> Post([FromRoute]string ChartName, string TaskName)
        {
            WFTask incomeTask = new WFTask {TaskId = 0, ChartName = ChartName, TaskName = TaskName};
            WFTask resp = await _taskRepository.Save(incomeTask);
            return Ok(resp);
        }

        // Put
        [HttpPut("{ChartName}/Task/{TaskName}/Description/{Description}", Name = "Put")]
        public async Task<IActionResult> Put([FromRoute]string ChartName, string TaskName, string Description)
        {
            WFTask incomeTask = new WFTask { TaskId = 0, ChartName = ChartName, TaskName = TaskName, TaskDescription = Description };
            WFTask resp = await _taskRepository.Save(incomeTask);
            return Ok(resp);
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{ChartId}/Task/{TaskId}", Name = "DeleteById")]
        public async Task<IActionResult> DeleteById(int ChartId, int TaskId)
        {
            //return NotFound(string.Format("Not found Chart '{0}' task '{1}'", ChartName, TaskName));
            //WFTask incomeTask = new WFTask { TaskId = 0, ChartName = ChartName, TaskName = TaskName };
            //var resp = await _taskRepository.Save(incomeTask);
            await _taskRepository.DeleteById(ChartId, TaskId);
            return Ok();
        }
    }
}
