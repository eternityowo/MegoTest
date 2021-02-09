using MegoTest.Data.Entities.Keyless;
using MegoTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MegoTest.Service.Interfaces
{
    public interface IMetricsService
    {
        public Task<IEnumerable<MetricStat>> GetMetricsAsync();

        public Task SaveMetricsAsync(IEnumerable<MetricModel> metricModels);
    }
}
