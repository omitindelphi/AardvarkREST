using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AardvarkREST.Models
{
    public interface IWFRouteRepository
    {
        Task<WFRoute> Get(string ChartName, string TaskFrom, string TaskTo);
        Task<WFRoute> Save(WFRoute Route);
        Task DeleteByNames(string ChartName, string TaskNameFrom, string TaskNameTo);
        Task<IEnumerable<WFRoute>> FindAll(string ChartName);
    }
}
