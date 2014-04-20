using System;
using Raven.Imports.Newtonsoft.Json;

namespace iMenyn.Data.Models
{
    public class LogEvent
    {
        public string FormattedMessage { get; set; }
        public string Level { get; set; }
        public string Exception { get; set; }
        public string InnerException { get; set; }
        public DateTime TimeStamp { get; set; }

        [JsonIgnore]
        public string FormatedTimeStamp
        {
            get { return TimeStamp.ToString("yy-MM-dd HH:mm:ss"); }
        }
    }
}