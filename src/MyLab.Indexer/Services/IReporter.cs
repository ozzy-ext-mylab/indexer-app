using System;
using Microsoft.Extensions.Logging;
using MyLab.LogDsl;

namespace MyLab.Indexer.Services
{
    interface IReporter
    {
        void ReportAboutLostEntities(string[] lostIds);
    }

    class DefaultReporter : IReporter
    {
        private readonly DslLogger _log;

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultReporter"/>
        /// </summary>
        public DefaultReporter(ILogger logger)
        {
            _log = logger.Dsl() ?? throw new ArgumentNullException(nameof(logger));
        }
        public void ReportAboutLostEntities(string[] lostIds)
        {
            _log.Warning("Detect lost entities")
                .AndFactIs("Lost entities", lostIds)
                .Write();
        }
    }
}