using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AchtungStarter.Core
{
    public class StateManager
    {
        private readonly StateStorage _stateStorage;

        public StateManager(StateStorage stateStorage)
        {
            _stateStorage = stateStorage;
        }

        public async Task<State?> LoadState()
        {
            await Task.CompletedTask;

            return _stateStorage.LoadState();
        }
    }
}
