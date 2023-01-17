using System;

namespace Delfinovin
{
    public class ControllerConnectionStatusEventArgs
    {
        public ConnectionStatus ConnectionStatus { get; set; }
        public ControllerType ControllerType { get; set; }
        public int ControllerPort { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
