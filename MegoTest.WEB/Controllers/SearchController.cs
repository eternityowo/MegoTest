using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MegoTest.Common;
using MegoTest.DAL.Entities;
using MegoTest.DAL.Models;
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
                var completeTasks = new List<string>();

                var A = _externalA.Request(source);
                var B = _externalB.Request(source);
                var C = _externalC.Request(source);
                Task<(RequestStatusCode status, int time, string name)> D = default;

                var externalTasks = new List<Task> { A, B, C };

                while (externalTasks.Count > 0)
                {
                    Task finishedTask = await Task.WhenAny(externalTasks);

                    if (finishedTask == A && A.Result.status != RequestStatusCode.TIMEOUT)
                    {
                        completeTasks.Add(TaskFormat(A.Result, A.Result.name));
                    }
                    else if (finishedTask == B && B.Result.status != RequestStatusCode.TIMEOUT)
                    {
                        completeTasks.Add(TaskFormat(B.Result, B.Result.name));
                    }
                    else if (finishedTask == C && C.Result.status != RequestStatusCode.TIMEOUT)
                    {
                        completeTasks.Add(TaskFormat(C.Result, C.Result.name));

                        if (C.Result.status == RequestStatusCode.OK)
                        {
                            D = _externalD.Request(source);
                            await D;

                            if (D.Result.status != RequestStatusCode.TIMEOUT)
                            {
                                completeTasks.Add(TaskFormat(D.Result, D.Result.name));
                            }
                        }
                    }
                    externalTasks.Remove(finishedTask);
                }

                var metics = new List<Metric>
                {
                    new Metric() { TaskName = A.Result.name, TimeInMs = A.Result.time },
                    new Metric() { TaskName = B.Result.name, TimeInMs = B.Result.time },
                    new Metric() { TaskName = C.Result.name, TimeInMs = C.Result.time }
                };

                if (C.Result.Item1 == RequestStatusCode.OK)
                {
                    metics.Add(new Metric() { TaskName = D.Result.name, TimeInMs = D.Result.time });
                }

                await _metrics.SaveMetricsAsync(metics);

                return completeTasks;
            }
        }

        [HttpGet]
        [Route("metrics")]
        public async Task<IEnumerable<MetricStat>> MetricsAsync()
        {
            var result = await _metrics.GetMetricsAsync();
            return result;
        }

        private string TaskFormat((RequestStatusCode status, int time, string name) result, string id)
        {
            return $"task: {id}, millisecond: {result.time}, status {result.status}";
        }
    }
}
