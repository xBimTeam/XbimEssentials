using System;
using System.Diagnostics;

namespace Xbim.Common.Logging
{
    [DebuggerDisplay("Level: {EventLevel} Logger: {Logger}.{Method} Message: {Message}")]
    public class Event
    {
        public DateTime EventTime { get; internal set; }
        public EventLevel EventLevel { get; internal set; }
        public String Message { get; internal set; }
        public String User { get; internal set; }
        public String Logger { get; internal set; }
        public String Method { get; internal set; }

    }
}
