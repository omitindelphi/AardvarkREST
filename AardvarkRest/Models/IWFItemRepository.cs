using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AardvarkREST.Models
{
    public interface IWFItemRepository
    {
        Task<IEnumerable<WFItemStatus>> GetAll(string ChartName, string ItemName);
        Task<WFItemStatus> ItemPutDown(string ChartName, string TaskName, string ItemName, string RouteCode, WFItemTaskStatusValue NewItemStatus);
        Task<WFItemStatus> ItemGetOut(string ChartName, string TaskName, string ItemName);
    }
}
