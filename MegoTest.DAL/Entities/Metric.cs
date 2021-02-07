using System;
using System.Collections.Generic;
using System.Text;

namespace MegoTest.DAL.Entities
{
    public class Metric : BaseEntity
    {
        public string TaskName { get; set; }
        public int TimeInMs { get; set; }
    }
}
