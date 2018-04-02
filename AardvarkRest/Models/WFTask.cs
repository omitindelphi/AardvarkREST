using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AardvarkREST.Models
{
    public class WFTask
    {
        [Key]
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }

        [ForeignKey("WFChart")]
        public string ChartName { get; set; }
    }
}
