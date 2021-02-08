using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AchtungStarter.Core
{
    public class ServerManager
    {
        private readonly StateStorage _stateStorage;
        private readonly AsyncLock _asyncLock = new AsyncLock();
        private readonly ContainerManager _containerManager;
        private readonly IOptions<ServerManagerOptions> _options;
        private readonly ShutDownManager _shutDownManager;
        private readonly ILogger<ServerManager> _logger;

        public ServerManager(
            StateStorage stateStorage,
            ContainerManager containerManager,
            IOptions<ServerManagerOptions> options,
            ShutDownManager shutDownManager,
            ILogger<ServerManager> logger)
        {
            _logger = logger;
            _shutDownManager = shutDownManager;
            _options = options;
            _containerManager = containerManager;
            _stateStorage = stateStorage;
        }

        public async Task<State?> StartServer()
        {
            using var asyncLock = await _asyncLock.LockAsync();
            var state = _stateStorage.LoadState();

            if (state is { })
            {
                return state;
            }

            var serverId = new Random().Next();
            state = new State(
                ServerId: serverId,
                Url: null,
                StartedTime: null,
                ShutDownTime: DateTimeOffset.Now.AddMinutes(5),
                ShutDownRunName: null);
            _stateStorage.SaveState(state);
            state = await _shutDownManager.ScheduleShutDown(state, GetDefaultShutDownTime());

            if (state is null)
            {
                throw new Exception($"State is null.");
            }

            var url = await _containerManager.CreateContainer(serverId);

            _logger.LogWarning("Started Achtung server #{serverId}.", serverId);

            return _stateStorage.SaveState(state with
            {
                Url = url,
                StartedTime = DateTimeOffset.Now,
            });
        }

        public async Task<State?> AddMinutes(int minutes)
        {
            using var asyncLock = await _asyncLock.LockAsync();
            var state = _stateStorage.LoadState();

            if (state?.ShutDownTime is null)
            {
                return state;
            }

            var newShutDownTime = state.ShutDownTime.AddMinutes(minutes);
            var maxShutDownTime = GetMaxShutDownTime();

            if (newShutDownTime > maxShutDownTime)
            {
                newShutDownTime = maxShutDownTime;
            }

            // If there is less than one minute difference then do not nothing.
            if (newShutDownTime < state.ShutDownTime.AddMinutes(1))
            {
                return state;
            }

            return _stateStorage.SaveState(await _shutDownManager.ScheduleShutDown(state, newShutDownTime));
        }

        public async Task<State?> ShutDownServer()
        {
            using var asyncLock = await _asyncLock.LockAsync();
            var state = _stateStorage.LoadState();

            if (state is null)
            {
                return null;
            }

            return _stateStorage.SaveState(await _shutDownManager.ShutDownNow(state));
        }

        private static DateTimeOffset GetMaxShutDownTime()
        {
            return DateTimeOffset.Now.AddHours(1);
        }

        private DateTimeOffset GetDefaultShutDownTime()
        {
            var minutes = _options.Value.DefaultLifetimeMinutes;

            if (minutes <= 1)
            {
                minutes = 10;
            }

            return DateTimeOffset.Now.AddMinutes(minutes);
        }
    }
}
