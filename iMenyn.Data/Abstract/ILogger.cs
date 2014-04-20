using System;
using System.Collections.Generic;
using iMenyn.Data.Models;

namespace iMenyn.Data.Abstract
{
    public interface ILogger
    {
        void Debug(string message, params object[] args);
        void Info(string message, params object[] args);
        void Warn(string message, params object[] args);
        void Error(string message, params object[] args);
        void Error(string message, Exception exception);
        void Fatal(string message, Exception exception);

        IEnumerable<LogEvent> GetLogList(string query, bool info, bool debug, bool error, bool fatal, bool warn);

        int CountLogItems();

        void DeleteOldLogitems();
    }
}