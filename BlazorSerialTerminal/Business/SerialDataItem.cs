using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorSerialTerminal.Business
{
    public class SerialDataItem
    {
        public SerialDataItem(DateTime time, string message)
        {
            Timestamp = time;
            DataString = message;
        }

        public DateTime Timestamp { get; set; }
        public string DataString { get; set; }
    }
}
