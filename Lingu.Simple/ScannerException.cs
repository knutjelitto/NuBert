using System;
using System.Collections.Generic;
using System.Text;

namespace Lingu.Simple
{
    public class ScannerException : ApplicationException
    {
        public ScannerException(string message) : base(message)
        {
        }
    }
}
