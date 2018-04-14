using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AardvarkREST.Models
{
    public class WFRoute
    {
        public int ChartId { get; set; }
        [Key]
        public int RouteId { get; set; }
        public string ChartName { get; set; }
        public string TaskFrom { get; set; }
        public string TaskTo { get; set; }
        public string RouteCode { get; set; }

        public static string DefaultRouteCode = "Ok"; 
    }
}
