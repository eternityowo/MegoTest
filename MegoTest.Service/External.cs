using MegoTest.Common;
using MegoTest.DAL.Interfaces;
using MegoTest.DAL.Entities;
using MegoTest.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MegoTest.Service
{
    public abstract class External : IExternal
    {
        public async Task<(RequestStatusCode status, int time)> Request(CancellationTokenSource source)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                source.Token.ThrowIfCancellationRequested();

                await Task.Delay(TimeSpan.FromMilliseconds(Rnd.TaskTimeMilliseconds), source.Token);
            }
            catch (TaskCanceledException ex)
            {
            }

            var status = source.Token.IsCancellationRequested ? RequestStatusCode.TIMEOUT : Rnd.TaskStatus;

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            var timeInMs = (int) Math.Round(ts.TotalMilliseconds, MidpointRounding.AwayFromZero);

            return (status, timeInMs);
        }
    }
}
