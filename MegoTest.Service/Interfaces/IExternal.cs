using MegoTest.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MegoTest.Data.Models;

namespace MegoTest.Service.Interfaces
{
    public interface IExternal
    {
        Task<MetricModel> Request(CancellationTokenSource source);
    }
}
