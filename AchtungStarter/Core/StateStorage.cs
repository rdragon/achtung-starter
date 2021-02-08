using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AchtungStarter.Core
{
    public class StateStorage
    {
        private readonly IOptions<StateStorageOptions> _options;

        public StateStorage(IOptions<StateStorageOptions> options)
        {
            _options = options;
        }

        private readonly object _lock = new object();

        public State? LoadState()
        {
            var path = Path;
            string json;

            lock (_lock)
            {
                if (!File.Exists(path))
                {
                    return null;
                }

                json = File.ReadAllText(path);
            }

            var state = JsonSerializer.Deserialize<State?>(json);

            return state?.ShutDownTime <= DateTimeOffset.Now ? null : state;
        }

        public State? SaveState(State? state)
        {
            var path = Path;
            var json = JsonSerializer.Serialize(state);

            if (System.IO.Path.GetDirectoryName(path) is string folder)
            {
                Directory.CreateDirectory(folder);
            }

            lock (_lock)
            {
                File.WriteAllText(path, json);
            }

            return state;
        }

        private string Path => _options.Value.StateLocation.GetSetting(921618);
    }
}
