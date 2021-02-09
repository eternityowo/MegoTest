using System;
using System.Collections.Generic;
using System.Text;

namespace MegoTest.Data.Entities.Keyless
{
    public class Metric : BaseEntity
    {
        public string TaskName { get; set; }
        public int TimeInMs { get; set; }
    }
}
