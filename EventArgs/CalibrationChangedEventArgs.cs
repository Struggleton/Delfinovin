using System;

namespace Delfinovin
{
    public class CalibrationChangedEventArgs : EventArgs
    {
        public CalibrationStatus CalibrationStatus { get; set; }
        public int ControllerPort { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
