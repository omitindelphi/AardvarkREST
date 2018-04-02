using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AardvarkREST.Models
{
    interface IWFTaskRepository
    {
        Task<WFTask> Get(string ChartName, string TaskName);
        Task<WFTask> Save(WFTask Task);
        //Task Delete(string ChartName, string TaskName);
        Task DeleteById(string ChartName,int TaskId);
        //Task Update(WFChart Chart);
        Task<IEnumerable<WFTask>> FindAll();
    }
}
