using MegoTest.DAL.Interfaces;
using MegoTest.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System;
using MegoTest.Data.Models;
using MegoTest.Data.Entities.Keyless;

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
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // in 1-2 ms faster on average + IQueryable
            var res = await _uow.GetDbSet<MetricStat>().FromSqlRaw(
                " DECLARE @TimeInterval int; SET @TimeInterval = 1000; " +
                " SELECT TaskName AS Name,  " +
                " CONCAT(TimeInMs / @TimeInterval, '-', TimeInMs / @TimeInterval + 1) AS Range, " +
                " COUNT(*) AS Count " +
                " FROM Metrics " +
                " GROUP BY TaskName, TimeInMs / @TimeInterval " +
                " ORDER BY TaskName, Range ").AsNoTracking().ToListAsync();

            //var dbRes = await _uow.GetRepository<Metric>().All().AsNoTracking().ToListAsync();
            //// The LINQ expression could not be translated
            //// change to raw sql or excute stored procedure
            //var res = dbRes
            //    .GroupBy(x => new
            //    {
            //        name = x.TaskName,
            //        value = (Enumerable.Range(1, 10).Select(x => x * 1000).FirstOrDefault(r => r >= x.TimeInMs) - 1000) / 1000
            //    })
            //    .Select(g1 => new MetricStat()
            //    {
            //        Name = g1.Key.name,
            //        Range = $"{g1.Key.value}-{g1.Key.value + 1} ms",
            //        Count = g1.Count()
            //    })
            //    .OrderBy(x => x.Name)
            //    .ToList();

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            var timeInMs = (int)Math.Round(ts.TotalMilliseconds, MidpointRounding.AwayFromZero);

            Console.WriteLine(timeInMs);

            return res;
        }

        public async Task SaveMetricsAsync(IEnumerable<MetricModel> metricModels)
        {
            var metrics = metricModels.Select(m => new Metric() { TaskName = m.TaskName, TimeInMs = m.TimeInMs }); // change to AutoMapper 
            await _uow.GetRepository<Metric>().InsertAsync(metrics);
            await _uow.SaveChangesAsync();
        }
    }
}
