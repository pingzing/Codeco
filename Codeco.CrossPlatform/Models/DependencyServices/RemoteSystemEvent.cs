using System;
using System.Collections.Generic;
using System.Text;

namespace Codeco.CrossPlatform.Models.DependencyServices
{
    public class RemoteSystemEvent
    {
        public string Id { get; set; }
        public RemoteEventKind EventKind { get; set; }
        public RemoteSystem System { get; set; }
    }    
}
