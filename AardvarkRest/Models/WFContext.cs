using Microsoft.EntityFrameworkCore;

namespace AardvarkREST.Models
{
    public partial class WFContext : DbContext
    {
        private IWFChartRepository _wFChartRepository;

        public virtual DbSet<WFChart> WFChart { get; set; }

        public virtual IWFChartRepository WFChartRepository { get => _wFChartRepository; set => _wFChartRepository = value; }


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
    }
}
