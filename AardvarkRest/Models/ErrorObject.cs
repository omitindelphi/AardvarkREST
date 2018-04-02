using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AardvarkREST.Models
{
    public class ErrorObject : IErrorObject
    {
        private string _errorMessage;
        private string _sqlErrorMessage;
        private string _callStack;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }
        public string SQLErrorMessage
        { get { return _sqlErrorMessage; }
          set { _sqlErrorMessage = value; }
        }
        public string CallStack
        { get { return _callStack; }
          set { _callStack = value; }
        }

        public ErrorObject( Exception e)
        {
            this.ErrorMessage = e.Message;
            this.SQLErrorMessage = "";
            this.CallStack = e.StackTrace;
        }
    }
}
