using System.Collections.Generic;
using System.Linq;
using Aicup2020.Game;
using Aicup2020.Model;

namespace Aicup2020
{
    public class MyStrategy
    {
        public Action GetAction(PlayerView playerView, DebugInterface debugInterface)
        {
            Log.DebugInterface = debugInterface;
            Log.Clear();
            World.Update(playerView);
            Bot.Do();
            Dictionary<int, EntityAction> actions = World.All(e => e.My).ToDictionary(
                k => k.Id,
                v => v.Action);

            return new Action(actions);
        }

        public void DebugUpdate(PlayerView playerView, DebugInterface debugInterface)
        {
            debugInterface.Send(new DebugCommand.Clear());
            debugInterface.GetState();
        }
    }
}