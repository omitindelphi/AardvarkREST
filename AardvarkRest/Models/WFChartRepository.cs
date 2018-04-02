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

        private string ChartNameSelectTail(string ChartName)
        {
            string result = "where ChartName is null"; // always return empty dataset by table definitions
            if ( ! string.IsNullOrEmpty(ChartName) )
            {
               result = string.Format(" where ChartName = '{0}' ", ChartName);
            }
            return result;
        }

        public async Task<WFChart> Get(string ChartName)
        {
            WFChart result = null;

            result = await _WFContext.WFChart.FromSql("select ChartName, ChartId, ChartDescription from owf.wfChart " + ChartNameSelectTail(ChartName)
                                                     ).SingleOrDefaultAsync<WFChart>(); 
            return result;
        }

        public async Task<WFChart> Save(WFChart Chart)
        {
            var id = Chart.ChartId;
            var ChartName = new SqlParameter("ChartName", Chart.ChartName);

            string chartDesc = "";
            if (Chart.ChartDescription != null) chartDesc = Chart.ChartDescription;

            var ChartDescription = new SqlParameter("ChartDescription", chartDesc); // cannot pass null parameter as Text value to SQL
            string selectCommandScript = " declare @id int;" +
                                    " exec owf.ChartSet @ChartName, @ChartDescription, @id out;" +
                                    " select ChartId, ChartName, ChartDescription from owf.WFChart where ChartId=@id";
            SqlParameter[] arrParam = { ChartName, ChartDescription };

            /*
            var connection = new SqlConnection(_WFContext.Database.GetDbConnection().ConnectionString);
            var command = new SqlCommand(selectCommandScript, arrParam, connection);
            
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader[0]}:{reader[1]} ${reader[2]}");
                }
            }
            */

            var ret = await _WFContext.WFChart.FromSql(selectCommandScript, arrParam).FirstOrDefaultAsync<WFChart>();
            return ret;

        }

        public async Task<IEnumerable<WFChart>> FindAll()
        {
            return await _WFContext.WFChart.FromSql("select ChartName, ChartId, ChartDescription from owf.wfChart ").ToListAsync();  
        }

        public async Task Delete(WFChart chart)
        {
            var ChartName = new SqlParameter("ChartName",chart.ChartName);
            var ret = await _WFContext.Database.ExecuteSqlCommandAsync("delete from owf.WFChart where chartName = @ChartName", ChartName);
            //var ret = await _WFContext.WFChart.FromSql("delete from owf.WFChart where chartName = @ChartName; select ChartName, ChartId, ChartDescription from owf.WFChart where chartName = @ChartName").FirstOrDefaultAsync<WFChart>();
            if (ret != 1)
                throw new SQLWFException(string.Format("Failure to delete \"{0}\" chart - returned value {1} from Delete routine", chart.ChartName, ret));
        }

        public async Task DeleteById(int id)
        {
            var ChartId = new SqlParameter("ChartId", id);
            var ret = await _WFContext.Database.ExecuteSqlCommandAsync("delete from owf.WFChart where ChartId = @ChartId", ChartId);
            if (ret != 1)
                throw new SQLWFException(string.Format("Failure to delete ChartId=\"{0}\" chart - returned value {1} from DeleteById routine", id, ret));
        }

    }
}
