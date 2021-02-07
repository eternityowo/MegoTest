using MegoTest.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MegoTest.Service.Interfaces
{
    public interface IExternal
    {
        Task<(RequestStatusCode status, int time)> Request(CancellationTokenSource source);
    }
}
