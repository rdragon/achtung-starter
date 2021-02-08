using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AchtungStarter.Core
{
    public record State(
        int ServerId,
        string? Url,
        DateTimeOffset? StartedTime,
        DateTimeOffset ShutDownTime,
        string? ShutDownRunName)
    {
        public bool Started => Url is { };
    }
}
