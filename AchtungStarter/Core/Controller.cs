using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AchtungStarter.Core
{
    public class Controller
    {
        private readonly StateStorage _stateStorage;
        private readonly ServerManager _serverManager;
        private readonly ILogger<Controller> _logger;

        public Controller(StateStorage stateStorage, ServerManager serverManager, ILogger<Controller> logger)
        {
            _logger = logger;
            _serverManager = serverManager;
            _stateStorage = stateStorage;
        }

        public State? GetState()
        {
            return _stateStorage.LoadState();
        }

        public void StartServer()
        {
            Task.Run(async () =>
            {
                try
                {
                    await _serverManager.StartServer();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception during 'Task.Run'.");
                }
            });
        }

        public async Task<State?> ShutDownServer()
        {
            return await _serverManager.ShutDownServer();
        }

        public async Task<State?> AddMinutes(int minutes)
        {
            if (minutes < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(minutes));
            }

            return await _serverManager.AddMinutes(minutes);
        }
    }
}
