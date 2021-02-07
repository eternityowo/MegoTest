using MegoTest.DAL.Entities;
using MegoTest.DAL.Models;
using MegoTest.DAL.Interfaces;
using MegoTest.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MegoTest.Service
{
    public class MetricsService : IMetricsService
    {
        public IUnitOfWork _uow;

        public MetricsService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<MetricStat>> GetMetricsAsync()
        {
            var dbRes = await _uow.GetRepository<Metric>()
                .All().ToListAsync();

            // The LINQ expression could not be translated
            // change to raw sql or excute stored procedure
            var res = dbRes
                .GroupBy(x => new
                {
                    name = x.TaskName,
                    value = (Enumerable.Range(1, 10).Select(x => x * 1000).FirstOrDefault(r => r >= x.TimeInMs) - 1000) / 1000
                })
                .Select(g1 => new MetricStat()
                {
                    Name = g1.Key.name,
                    Range = $"{g1.Key.value}-{g1.Key.value + 1} ms",
                    Count = g1.Count()
                })
                .OrderBy(x => x.Name)
                .ToList();

            return res;
        }

        public async Task SaveMetricsAsync(IEnumerable<Metric> metrics)
        {
            await _uow.GetRepository<Metric>().InsertAsync(metrics);
            await _uow.SaveChangesAsync();
        }
    }
}
