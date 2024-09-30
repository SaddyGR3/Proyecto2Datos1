using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryProcessor.Exceptions
{
    public class SQLParserException : Exception
    {
        public SQLParserException() : base()
        {
        }

        public SQLParserException(string message) : base(message)
        {
        }

        public SQLParserException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

