using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AardvarkREST.Models;

namespace AardvarkREST.Models
{
    public class WFTaskRepository: IWFTaskRepository
    {
        private WFContext _WFContext;


        public WFTaskRepository(WFContext existingContext)
        {
            _WFContext = existingContext;
        }

        /*
        private string ChartNameSelectTail(string ChartName)
        {
            string result = "where ChartName is null"; // always return empty dataset by table definitions
            if ( ! string.IsNullOrEmpty(ChartName) )
            {
               result = string.Format(" where ChartName = '{0}' ", ChartName);
            }
            return result;
        }
        */

        public async Task<WFTask> Get(string ChartName, string TaskName)
        {
            WFTask result = null;
            result = await _WFContext.WFTaskGetByName(ChartName, TaskName);
            return result;
        }

        public async Task<WFTask> Save(WFTask Task)
        {
            var ret = await _WFContext.WFTaskSave(Task);
            return ret;
        }

        public async Task<IEnumerable<WFTask>> FindAll(string ChartName)
        {
            return await _WFContext.WFTaskFindAll(ChartName); 
        }

        public async Task DeleteById(int ChartId, int TaskId)
        {
            await _WFContext.WFTaskDeleteById(ChartId, TaskId);
        }

        public async Task DeleteByName(string ChartName, string TaskNameFrom, string TaskNameTo)
        {
            await _WFContext.WFRouteDeleteByName(ChartName, TaskNameFrom, TaskNameTo);
        }

    }
}
