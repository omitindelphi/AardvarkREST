using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AardvarkREST.Models
{
    public class WFItemRepository: IWFItemRepository
    {
        private WFContext _WFContext;

        public WFItemRepository(WFContext existingContext)
        {
            _WFContext = existingContext;
        }

        public async Task<WFItemStatus> ItemPutDown(string ChartName, string TaskName, string ItemName, string RouteCode, WFItemTaskStatusValue NewItemStatus)
        {
            var result = await _WFContext.WFNavigationItemPut(ChartName, TaskName, ItemName, RouteCode, NewItemStatus);
            return result;
        }

        public async Task<IEnumerable<WFItemStatus>> GetAll(string ChartName, string ItemName)
        {
            var result = await _WFContext.WFItemStatusByName(ChartName, ItemName);
            return result;
        }

        public async Task<WFItemStatus> ItemGetOut(string ChartName, string TaskName, string ItemName)
        {
            var result = await _WFContext.WFNavigationItemGet(ChartName, TaskName, ItemName);
            return result;
        }
    }
}
