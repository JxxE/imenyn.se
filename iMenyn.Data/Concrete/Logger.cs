using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Abstractions.Data;
using Raven.Client;
using NLog;
using iMenyn.Data.Abstract;
using iMenyn.Data.Infrastructure;
using iMenyn.Data.Infrastructure.Index;
using iMenyn.Data.Models;

namespace iMenyn.Data.Concrete
{
    public class Logger : ILogger
    {
        private IDocumentStore _documentStore;

        private IDocumentStore DocumentStore
        {
            get
            {
                if (_documentStore == null)
                    _documentStore = DependencyManager.GetInstance<IRavenDbContext>().DocumentStore;
                return _documentStore;
            }
        }

        #region Implementation of ILogger

        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public void Debug(string message, params object[] args)
        {
            logger.Debug(message, args);
        }

        public void Info(string message, params object[] args)
        {
            logger.Info(message, args);
        }

        public void Warn(string message, params object[] args)
        {
            logger.Warn(message, args);
        }

        public void Error(string message, params object[] args)
        {
            logger.Error(message, args);
        }

        public void Error(string message, Exception exception)
        {
            logger.ErrorException(message, exception);
        }

        public void Fatal(string message, Exception exception)
        {
            if (!ExcludeFomLog(message))
                logger.FatalException(message, exception);
        }

        public IEnumerable<LogEvent> GetLogList(string query, bool info, bool debug, bool error, bool fatal, bool warn)
        {
            using (var session = DocumentStore.OpenSession())
            {
                var loglist = session.Query<LogEvent, LogEvents>().Where(x => x.FormattedMessage.StartsWith(query));

                if (!info)
                    loglist = loglist.Where(l => l.Level != "Info");
                if (!debug)
                    loglist = loglist.Where(l => l.Level != "Debug");
                if (!error)
                    loglist = loglist.Where(l => l.Level != "Error");
                if (!fatal)
                    loglist = loglist.Where(l => l.Level != "Fatal");
                if (!warn)
                    loglist = loglist.Where(l => l.Level != "Warn");

                return loglist.OrderByDescending(l => l.TimeStamp).Take(1000);
            }

        }

        public int CountLogItems()
        {
            using (var session = DocumentStore.OpenSession())
                return session.Query<LogEvent>().Count();
        }

        private bool ExcludeFomLog(string message)
        {
            // Exception caused by UptimeRobot
            if (message.StartsWith("Unhandled exception: A public action method '"))
                return true;

            return false;
        }

        public void DeleteOldLogitems()
        {
            IDocumentQuery<LogEvent> query;
            using (var session = DocumentStore.OpenSession())
            {
                query = session.Advanced.LuceneQuery<LogEvent, LogEvents>().WhereBetween("TimeStamp", DateTime.MinValue, DateTime.Now.AddDays(-7));
            }

            DocumentStore.DatabaseCommands.DeleteByIndex(typeof(LogEvents).Name, new IndexQuery
            {
                Query = query.ToString()
            }, allowStale: true);
        }

        #endregion
    }
}