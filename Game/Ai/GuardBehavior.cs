using System.Collections.Generic;
using Aicup2020.Game;

namespace aicup2020.Game.Ai
{
    public static class GuardBehavior
    {
        public static void Guard(List<Entity> army)
        {
            if(army.Count < 5)
            {
                army.RemoveAll(e => e.Position.X < 25 &&
                                    e.Position.Y < 25);
            }
        }
    }
}