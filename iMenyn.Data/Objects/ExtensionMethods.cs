using System;
using System.Threading;
using Raven.Client;

namespace iMenyn.Data.Objects
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Waits until RavenDB has no stale indexes, only use this if you know what you're doing.
        /// This method can be SLOW!
        /// It allows RavenDB to wait for 5 minutes to be non stale, if it doesn't succeed an exception will be thrown.
        /// </summary>
        /// <param name="db">IDocumentStore</param>
        public static void ClearStaleIndexes(this IDocumentStore db)
        {
            var counter = 0;

            while (db.DatabaseCommands.GetStatistics().StaleIndexes.Length != 0 && counter < 3000)
            {
                Thread.Sleep(100);
                counter++;
            }

            if (db.DatabaseCommands.GetStatistics().StaleIndexes.Length > 0)
                throw new TimeoutException("RavenDB was unable to wait for non stale results");

        }
    }
}