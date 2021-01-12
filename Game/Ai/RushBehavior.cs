using System.Collections.Generic;
using System.Linq;
using Aicup2020.Game;

namespace aicup2020.Game.Ai
{
    public static class RushBehavior
    {
        public static void Rush(List<Entity> units, List<Entity> enemies)
        {
            foreach (Entity unit in units.ToList())
            {
                EntityTarget target = Helper.GetNearest(unit.Position, enemies);
                if (target == null)
                {
                    continue;
                }

                Entity enemy = target.Entity;
                MoveHelper.Move(unit, enemy.Position);
                units.Remove(unit);
            }
        }
    }
}