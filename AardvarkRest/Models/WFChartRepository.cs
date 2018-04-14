using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AardvarkREST.Models
{
    public class WFChartRepository : IWFChartRepository
    {
        private WFContext _WFContext;


        public WFChartRepository(WFContext existingContext)
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

        public async Task<WFChart> Get(string ChartName)
        {
            WFChart result = null;
            result = await _WFContext.WFChartGetByName(ChartName);
            return result;
        }

        public async Task<WFChart> Save(WFChart Chart)
        {
            var ret = await _WFContext.WFChartSave(Chart);
            return ret;
        }

        public async Task<IEnumerable<WFChart>> FindAll()
        {
            return await _WFContext.WFChartFindAll(); //.FromSql("select ChartName, ChartId, ChartDescription from owf.wfChart ").ToListAsync();  
        }

        public async Task DeleteById(int id)
        {
           await _WFContext.WFChartDeleteById(id);
        }

    }
}
