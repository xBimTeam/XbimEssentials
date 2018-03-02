using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.Common.Step21
{
    public static class LogEventIds
    {
        public static EventId ParserFailure = new EventId(200, nameof(ParserFailure));
        public static EventId FailedPropertySetter = new EventId(201, nameof(FailedPropertySetter));
        public static EventId FailedEntity = new EventId(202, nameof(FailedEntity));
    }
}
