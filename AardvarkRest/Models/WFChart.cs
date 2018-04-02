using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AardvarkREST.Models
{
    public class WFChart
    {
        [Key]
        public int ChartId { get; set; }
        public string ChartName { get; set; }
        public string ChartDescription { get; set; }
    }
}
