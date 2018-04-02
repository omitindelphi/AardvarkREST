using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AardvarkREST.Models
{
    public class SQLWFException : Exception
    {
        public SQLWFException(string ErrorMessage): base(ErrorMessage)
        {
             
        }
    }
}
