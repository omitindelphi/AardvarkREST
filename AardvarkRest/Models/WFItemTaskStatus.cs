using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AardvarkREST.Models
{
    public enum WFItemTaskStatusValue
    {
        Ready = 0,
        InProcess = 1,
        Completed = 2,
        Rejected = 3,
        OnHold = 4,
        Reserve = 5
    }

    public class WFItem
    {  
       [Key]
       public int ItemId { get; set; }
       public string ChartName { get; set; }
       public string ItemName { get; set; }
    }

    public class WFItemStatus
    {
        [Key]
        public string sid { get;  set; }
        public Int64 ItemId { get; set; }
        public int TaskId { get; set; }
        public string ItemName { get; set; }
        public string TaskName { get; set; }
        public string ChartName { get; set; }
        public WFItemTaskStatusValue ItemTaskStatus { get; set; }
    }
}
