using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AchtungStarter.Core
{
    public class ShutDownManager
    {
        private readonly LogicAppManager _logicAppManager;
        private readonly ContainerManager _containerManager;

        public ShutDownManager(LogicAppManager logicAppManager, ContainerManager containerManager)
        {
            _containerManager = containerManager;
            _logicAppManager = logicAppManager;
        }

        public async Task<State?> ShutDownNow(State state)
        {
            return await ScheduleShutDown(state, DateTimeOffset.Now);
        }

        public async Task<State?> ScheduleShutDown(State state, DateTimeOffset shutDownTime)
        {
            // If there is already a shutdown scheduled around approximately the same time,
            // then do nothing.
            if (state.ShutDownRunName is { } &&
                state.ShutDownTime is DateTimeOffset foundTime &&
                Math.Abs(foundTime.Subtract(shutDownTime).TotalMinutes) < 1)
            {
                return shutDownTime <= DateTimeOffset.Now ? null : state;
            }

            var runName = await _logicAppManager.ScheduleShutDown(shutDownTime, _containerManager.GetContainerGroupName(state.ServerId));

            if (state.ShutDownRunName is string foundRunName)
            {
                await _logicAppManager.CancelShutDown(foundRunName);
            }

            return shutDownTime <= DateTimeOffset.Now ? null : state with { ShutDownRunName = runName, ShutDownTime = shutDownTime };
        }
    }
}
