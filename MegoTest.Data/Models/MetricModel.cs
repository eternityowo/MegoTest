using MegoTest.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MegoTest.Data.Models
{
    public class MetricModel
    {
        public string TaskName { get; set; }
        public int TimeInMs { get; set; }
        public RequestStatusCode Status { get; set; }

        public override string ToString()
        {
            return $"TaskName: {TaskName}, TimeInMs: {TimeInMs}, Status: {Status}";
        }
    }
}
