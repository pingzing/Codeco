using System;

namespace Codeco.Windows10.Services
{
    public class ServiceNotInitializedException : Exception
    {
        public ServiceNotInitializedException() { }
        public ServiceNotInitializedException(string message) : base(message) { }
        public ServiceNotInitializedException(string message, Exception inner) : base(message, inner) { }        
    }
}
