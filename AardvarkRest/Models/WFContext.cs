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
        public virtual DbSet<WFChart> WFChart { get; set; }

        public WFContext(DbContextOptions<WFContext> options)
        : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WFChart>(entity =>
            {
                entity.Property(e => e.ChartName).IsRequired();
            });
        }

        public virtual async Task<WFChart> WFChartGetByName(string ChartName)
        {
          return  await WFChart.FromSql("select ChartName, ChartId, ChartDescription from owf.wfChart " + ChartNameSelectTail(ChartName)
                                                     ).SingleOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<WFChart>> WFChartFindAll()
        {
            return await WFChart.FromSql("select ChartName, ChartId, ChartDescription from owf.wfChart ").ToListAsync();
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
    }
}
