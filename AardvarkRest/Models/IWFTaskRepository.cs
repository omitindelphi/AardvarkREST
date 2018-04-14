using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AardvarkREST.Models
{
    public interface IWFTaskRepository
    {
        Task<WFTask> Get(string ChartName, string TaskName);
        Task<WFTask> Save(WFTask Task);
        Task DeleteById(int ChartId,int TaskId);
        Task<IEnumerable<WFTask>> FindAll(string ChartName);
    }
}
