using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AardvarkREST.Models
{
    public interface IErrorObject
    {
        string ErrorMessage { get; set; }
        string SQLErrorMessage { get; set; }
        string CallStack { get; set; }
    }
}
