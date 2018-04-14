using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace AardvarkREST.Models
{
    public partial class WFContext : DbContext
    {
        protected virtual DbSet<WFChart> WFChartTab { get; set; }
        protected virtual DbSet<WFTask> WFTaskTab { get; set; }
        protected virtual DbSet<WFRoute> WFRouteTab { get; set; }

        public WFContext(DbContextOptions<WFContext> options)
        : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WFChart>(entity =>
            {
                entity.Property(e => e.ChartName).IsRequired();
            });

           // modelBuilder.Entity<WFTask>(entity =>
           // {
           //     entity.Property(e => e.TaskName).IsRequired();
           // });

        }

        public virtual async Task<WFChart> WFChartGetByName(string ChartName)
        {
          return  await WFChartTab.FromSql("select ChartName, ChartId, ChartDescription from owf.wfChart " + ChartNameSelectTail(ChartName)
                                                     ).SingleOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<WFChart>> WFChartFindAll()
        {
            return await WFChartTab.FromSql("select ChartName, ChartId, ChartDescription from owf.wfChart ").ToListAsync();
        }

        public async Task<WFChart> WFChartSave(WFChart Chart)
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


            var ret = await WFChartTab.FromSql(selectCommandScript, arrParam).FirstOrDefaultAsync<WFChart>();
            return ret;
        }

        public async Task WFChartDeleteById(int id)
        {
            var ChartId = new SqlParameter("ChartId", id);
            var ret = await Database.ExecuteSqlCommandAsync("delete from owf.WFChart where ChartId = @ChartId", ChartId);
            if (ret != 1)
                throw new SQLWFException(string.Format("Failure to delete ChartId=\"{0}\" chart - returned value {1} from DeleteById routine", id, ret));
        }

        private string ChartNameSelectTail(string ChartName)
        {
            string result = "where ChartName is null"; // always return empty dataset by table definitions
            if (!string.IsNullOrEmpty(ChartName))
            {
                result = string.Format(" where ChartName = '{0}' ", ChartName);
            }
            return result;
        }

        public virtual async Task<WFTask> WFTaskGetByName(string ChartName, string TaskName)
        {
            return await WFTaskTab.FromSql( 
                "select a.ChartId, a.TaskId,  b.ChartName, a.TaskName, a.TaskDescription from owf.wfTask a " +
                "inner join owf.wfChart b on a.ChartId = b.ChartId " + TaskNameSelectTail(ChartName, TaskName)
                                                       ).SingleOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<WFTask>> WFTaskFindAll(string ChartName)
        {
            return await WFTaskTab.FromSql(
                                         string.Format(
                "select a.ChartId, a.TaskId,  b.ChartName, a.TaskName, a.TaskDescription from owf.wfTask a " +
                "inner join owf.wfChart b on a.ChartId = b.ChartId where b.chartName = '{0}'" , ChartName
                                                       )).ToListAsync();
        }

        private string TaskNameSelectTail(string ChartName, string TaskName)
        {
            string result = "where b.ChartName is null and a.TaskName is null"; // always return empty dataset by table definitions
            if (!string.IsNullOrEmpty(ChartName) && !string.IsNullOrEmpty(TaskName))
            {
                result = string.Format(" where b.ChartName = '{0}' and a.TaskName ='{1}' ", ChartName, TaskName);
            }
            return result;
        }

        public async Task<WFTask> WFTaskSave(WFTask aTask)
        {
           
            SqlParameter qChartName = new SqlParameter("qChartName", aTask.ChartName);
            SqlParameter qTaskName = new SqlParameter("qTaskName", aTask.TaskName);

            string aTaskDesc = "";
            if (aTask.TaskDescription != null)
                 aTaskDesc = aTask.TaskDescription;
            SqlParameter qTaskDescription = new SqlParameter("qTaskDescription", aTaskDesc); // cannot pass null parameter as Text value to SQL

            string selectCommandScript = " declare @id int;" 
            + " exec owf.TaskSet @qChartName, @qTaskName, @qTaskDescription, @id out;" 
            + " select a.ChartId, a.TaskId, b.ChartName, a.TaskName, a.TaskDescription from owf.WFTask a inner join owf.WFChart b on a.ChartId = b.ChartId where a.TaskId=@id";
                                    ;
            SqlParameter[] arrParam = { qChartName, qTaskName, qTaskDescription };

            var ret = await WFTaskTab.FromSql(selectCommandScript, arrParam).FirstOrDefaultAsync<WFTask>();
            return ret;
        }

        public async Task WFTaskDeleteById(int ChartId, int TaskId)
        {
            
            var ChartNameParameter = new SqlParameter("ChartId", ChartId);
            var TaskNameParameter = new SqlParameter("TaskId", TaskId);

            var ret = await Database.ExecuteSqlCommandAsync("delete from owf.WFTask where TaskId = "
                +"(select a.TaskId from owf.wfTask a inner join owf.wfChart b on a.ChartId = b.ChartId where a.TaskId = @TaskId and b.ChartId = @ChartId) ", 
                 ChartNameParameter, TaskNameParameter);
            if (ret != 2)
                throw new SQLWFException(string.Format("Failure to delete ChartName=\"{0}\" TaskName = \"{1}\"- returned value {2} from DeleteById routine", ChartId,TaskId, ret));
            
        }


        public virtual async Task<WFRoute> WFRouteGetByName(string ChartName, string TaskNameFrom, string TaskNameTo)
        {
            return await WFRouteTab.FromSql(
               // "select a.ChartId, a.TaskId,  b.ChartName, a.TaskName, a.TaskDescription from owf.wfTask a " +
                "inner join owf.wfChart b on a.ChartId = b.ChartId " + RouteSelectTail(ChartName, TaskNameFrom, TaskNameTo)
                                                       ).SingleOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<WFRoute>> WFRouteFindAll(string ChartName)
        {
//        public int ChartId { get; set; }
//        public int RouteId { get; set; }
//        public string ChartName { get; set; }
//        public string TaskFrom { get; set; }
//        public string TaskTo { get; set; }
//        public string RouteCode { get; set; }

            return await WFRouteTab.FromSql(
                                         string.Format(
                  "select a.ChartId, a.RouteId, d.ChartName, b.TaskName as TaskFrom,  c.TaskName as TaskTo, a.RouteCode" 
                + " from owf.wfRoute a " 
                + " inner join owf.wfTask b on a.FromTaskId = b.TaskId "
                + " inner join owf.wfTask c on a.ToTaskId = c.TaskId"
                + " inner join owf.wfChart d on a.ChartId = d.ChartId where d.chartName = '{0}'", ChartName
                                                       )).ToListAsync();

        }

        private string RouteSelectTail(string ChartName, string TaskNameFrom, string TaskNameTo)
        {
            string result = "where b.ChartName is null and a.TaskName is null"; // always return empty dataset by table definitions
            if (!string.IsNullOrEmpty(ChartName) && !string.IsNullOrEmpty(TaskNameFrom) && !string.IsNullOrEmpty(TaskNameTo))
            {
                result = string.Format(" where b.ChartName = '{0}' and a.TaskNameFrom = '{1}' and a.TaskNameTo = '{2}'", ChartName, TaskNameFrom, TaskNameTo);
            }
            return result;
        }

        public async Task<WFRoute> WFRouteSave(WFRoute aRoute)
        {

            SqlParameter qChartName = new SqlParameter("Chart", aRoute.ChartName);
            SqlParameter qTaskFrom = new SqlParameter("TaskNameFrom", aRoute.TaskFrom);
            SqlParameter qTaskTo = new SqlParameter("TaskNameTo", aRoute.TaskTo);
            SqlParameter qRouteCode = new SqlParameter("RouteCode", aRoute.RouteCode);
            /*
                        string aTaskDesc = "";
                        if (aTask.TaskDescription != null)
                            aTaskDesc = aTask.TaskDescription;
                        SqlParameter qTaskDescription = new SqlParameter("qTaskDescription", aTaskDesc); // cannot pass null parameter as Text value to SQL
            */
            string selectCommandScript = " declare @id int;"
            + " exec owf.RouteSet @Chart, @TaskNameFrom, @TaskNameTo, @RouteCode, @id out;"
            + " select a.ChartId, a.RouteId, d.ChartName, b.TaskName as TaskFrom,  c.TaskName as TaskTo, a.RouteCode"
                + " from owf.wfRoute a "
                + " inner join owf.wfTask b on a.FromTaskId = b.TaskId "
                + " inner join owf.wfTask c on a.ToTaskId = c.TaskId"
                + " inner join owf.wfChart d on a.ChartId = d.ChartId where a.RouteId = @id"

            ;
            SqlParameter[] arrParam = { qChartName, qTaskFrom, qTaskTo, qRouteCode
                                      };

            var ret = await WFRouteTab.FromSql(selectCommandScript, arrParam
                                             ).FirstOrDefaultAsync<WFRoute>();
            return ret;
        }

        public async Task WFRouteDeleteById(int ChartId, int RouteId)
        {

            var ChartNameParameter = new SqlParameter("ChartId", ChartId);
            var TaskNameParameter = new SqlParameter("TaskId", RouteId);

            var ret = await Database.ExecuteSqlCommandAsync("delete from owf.WFRoute where RouteId = "
                //+ "(select a.RouteId from owf.wfTask a inner join owf.wfChart b on a.ChartId = b.ChartId where a.TaskId = @TaskId and b.ChartId = @ChartId) "
                // ChartNameParameter, TaskNameParameter
                 );
            if (ret != 2)
                throw new SQLWFException(string.Format("Failure to delete ChartName=\"{0}\" TaskName = \"{1}\"- returned value {2} from DeleteById routine", ChartId, RouteId, ret));

        }
    }
}
