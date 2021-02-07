using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace MegoTest.DAL
{
    public class ExtendedLog
    {

        protected ExtendedLog()
        {
        }

        public byte TaskId { get; set; }
        public int TimeInMs { get; set; }
    }
}
