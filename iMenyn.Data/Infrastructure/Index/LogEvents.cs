using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using iMenyn.Data.Models;

namespace iMenyn.Data.Infrastructure.Index
{
    public class LogEvents : AbstractIndexCreationTask<LogEvent>
    {
        public LogEvents()
        {
            Map = logList => from logEvent in logList
                             select new { logEvent.FormattedMessage, logEvent.Level, logEvent.TimeStamp };


            Indexes.Add(x => x.FormattedMessage, FieldIndexing.Analyzed);
            Indexes.Add(x => x.Level, FieldIndexing.NotAnalyzed);
        }

    }
}