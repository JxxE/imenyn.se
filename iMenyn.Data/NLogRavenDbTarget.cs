using NLog;
using NLog.Targets;
using Raven.Client;
using iMenyn.Data.Abstract;
using iMenyn.Data.Infrastructure;
using iMenyn.Data.Models;

namespace iMenyn.Data
{
    [Target("RavenDbTarget")]
    public class NLogRavenDbTarget : TargetWithLayout
    {
        private IDocumentStore DocumentStore;

        public NLogRavenDbTarget()
        {
        }

        public NLogRavenDbTarget(IDocumentStore documentStore)
        {
            DocumentStore = documentStore;
        }

        protected override void Write(LogEventInfo logEventInfo)
        {
            if (DocumentStore == null)
                DocumentStore = DependencyManager.GetInstance<IRavenDbContext>().DocumentStore;

            var logEvent = new LogEvent
            {
                FormattedMessage = logEventInfo.FormattedMessage,
                Level = logEventInfo.Level.Name,
                TimeStamp = logEventInfo.TimeStamp
            };
            // Set exceptions
            if (logEventInfo.Exception != null)
            {
                logEvent.Exception = logEventInfo.Exception.StackTrace;
                if (logEventInfo.Exception.InnerException != null)
                {
                    logEvent.InnerException = logEventInfo.Exception.InnerException.Message + "<br/>" + logEventInfo.Exception.InnerException.StackTrace;
                }
            }

            using (var session = DocumentStore.OpenSession())
            {
                session.Store(logEvent);
                session.SaveChanges();
            }
        }
    }
}