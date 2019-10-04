using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Configuration;

namespace CoreWebApp.Serilog
{
    public static class LoggingExtension
    {
        public static LoggerConfiguration WithAppInfo(this LoggerEnrichmentConfiguration enrich)
        {
            if (enrich == null)
            {
                throw new ArgumentNullException(nameof(enrich));
            }

            return enrich.With<AppInfoLogEnricher>();
        }
    }
}