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
    [Route("api/WFItem")]
    public class WFItemController : Controller
    {
        private IWFItemRepository _itemRepository;

        public WFItemController(IWFItemRepository existingItemRepository)
        {
            _itemRepository = existingItemRepository;
        }
        
        // GET: api/Item
        [HttpGet("{ChartName}/Item/{ItemName}", Name = "GetItemStatuses")]
        public IEnumerable<WFItemStatus> GetItemStatuses(string ChartName, string ItemName)
        {
            return _itemRepository.GetAll(ChartName, ItemName).Result;
        }

        [HttpPost("{ChartName}/TaskActionPut/{TaskName}/Item/{ItemName}", Name = "PutItemStatus")]
        public async  Task<IActionResult> PutItemStatus(string ChartName, string TaskName, string ItemName)
        {
            string RouteCode = "Ok";
            WFItemTaskStatusValue NewStatus = WFItemTaskStatusValue.Completed;

            var item = await _itemRepository.ItemPutDown(ChartName, TaskName, ItemName, RouteCode, NewStatus);

            return Ok(item);
        }

        [HttpPost("{ChartName}/TaskActionExtract/{TaskName}/Item/{ItemName}", Name = "ExtractItem")]
        public async Task<IActionResult> ExtractItem(string ChartName, string TaskName, string ItemName)
        {
            string RouteCode = "Ok";
            WFItemTaskStatusValue NewStatus = WFItemTaskStatusValue.Completed;
            var item = await _itemRepository.ItemGetOut(ChartName, TaskName, ItemName);
            return Ok(item);
        }

        /*
        // GET: api/Item/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/Item
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/Item/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        */

    }
}
