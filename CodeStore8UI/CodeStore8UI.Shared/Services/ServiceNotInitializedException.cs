using System;
using System.Collections.Generic;
using System.Text;

namespace Codeco.Services
{    
   public class ServiceNotInitializedException : Exception
    {
        public ServiceNotInitializedException() { }
        public ServiceNotInitializedException(string message) : base(message) { }
        public ServiceNotInitializedException(string message, Exception inner) : base(message, inner) { }        
    }
}
