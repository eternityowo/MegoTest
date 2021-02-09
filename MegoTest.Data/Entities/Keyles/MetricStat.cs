using Microsoft.EntityFrameworkCore;

namespace MegoTest.Data.Models
{
    [Keyless]
    public class MetricStat
    {
        public string Name { get; set; }
        public string Range { get; set; }
        public int Count { get; set; }
    }
}
