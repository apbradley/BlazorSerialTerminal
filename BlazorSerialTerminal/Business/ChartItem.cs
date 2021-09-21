using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorSerialTerminal.Business
{
    public class ChartItem
    {
        public double Value { get; set; }

        public string Label { get; set; }

        public ChartItem(string label, double value1)
        {
            Label = label;
            Value = value1;
        }
    }

    public class NodeChartData
    {
        public NodeChartData(DateTime time, double value1)
        {
            Timestamp = time;
            DataValue = value1;
        }

        public DateTime Timestamp { get; set; }
        public double DataValue { get; set; }
    }
    public class SingleDataValueItem
    {
        public string Label { get; set; }
        public double Value { get; set; }
        public SingleDataValueItem(string label, double value)
        {
            Label = label; Value = value;
        }

    }
}
