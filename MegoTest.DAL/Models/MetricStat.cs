using Microsoft.EntityFrameworkCore;

namespace MegoTest.DAL.Models
{
    [Keyless]
    public class MetricStat
    {
        public string Name { get; set; }
        public string Range { get; set; }
        public int Count { get; set; }
    }
}
