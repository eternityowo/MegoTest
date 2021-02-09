using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MegoTest.Common;
using MegoTest.Data.Entities.Keyless;
using MegoTest.Data.Models;
using MegoTest.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MegoTest.WEB.Controllers
{
    [ApiController]
    public class SearchController : Controller
    {
        private readonly ILogger<SearchController> _logger;

        private IConfiguration _config;

        private readonly IExternalA _externalA;
        private readonly IExternalB _externalB;
        private readonly IExternalC _externalC;
        private readonly IExternalD _externalD;

        public IMetricsService _metrics;

        public SearchController(
            ILogger<SearchController> logger, IConfiguration config,
            IExternalA externalA, IExternalB externalB, 
            IExternalC externalC, IExternalD externalD,
            IMetricsService metrics)
        {
            _logger = logger;

            _config = config;

            _externalA = externalA;
            _externalB = externalB;
            _externalC = externalC;
            _externalD = externalD;

            _metrics = metrics;
        }

        [HttpGet]
        [Route("search")]
        public async Task<IEnumerable<string>> SearchAsync()
        {
            int.TryParse(_config["TimeoutMilliseconds"], out var timeout);

            using (var source = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeout)))
            {
                var metics = new List<MetricModel>();

                var A = _externalA.Request(source);
                var B = _externalB.Request(source);
                var C = _externalC.Request(source);

                var externalTasks = new Dictionary<Task<MetricModel>, Action<MetricModel>>()
                {
                    { A, (result) => metics.Add(result) },
                    { B, (result) => metics.Add(result) },
                    { C, async (result) => 
                        {
                            metics.Add(result);

                            if (result.Status == RequestStatusCode.OK)
                            {
                                var D = _externalD.Request(source);
                                await D;
                                metics.Add(D.Result);
                            }
                        }
                    }
                };

                while (externalTasks.Count > 0)
                {
                    Task<MetricModel> finishedTask = await Task.WhenAny(externalTasks.Keys);

                    externalTasks[finishedTask].Invoke(finishedTask.Result);

                    externalTasks.Remove(finishedTask);
                }

                await _metrics.SaveMetricsAsync(metics);

                var res = metics.Where(m => m.Status != RequestStatusCode.TIMEOUT).Select(m => m.ToString());

                return res;
            }
        }

        [HttpGet]
        [Route("metrics")]
        public async Task<IEnumerable<MetricStat>> MetricsAsync()
        {
            var result = await _metrics.GetMetricsAsync();
            return result;
        }
    }
}
