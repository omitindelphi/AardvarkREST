using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AardvarkREST.Models
{
    public interface IWFChartRepository
    {
        Task<WFChart> Get(string ChartName);
        Task<WFChart> Save(WFChart Chart);
        Task DeleteById(int id);
        Task<IEnumerable<WFChart>> FindAll();

        //Task Delete(WFChart Chart);
        //Task Update(WFChart Chart);
    }
}
