using System.Collections.Generic;
using Aicup2020.Game;

namespace aicup2020.Game.Ai
{
    public static class DefenceBehavior
    {
        public static void Defence(List<Entity> units, List<Entity> enemies)
        {
            List<Entity> facilities = World.All(e => e.My && e.Facility);
            foreach (Entity facility in facilities)
            {
                List<EntityTarget> targets = Helper.GetNearestInRange(facility, enemies, 25);
                foreach (EntityTarget target in targets)
                {
                    Entity enemy = target.Entity;
                    Entity defender = Helper.GetNearest(enemy.Position, units)?.Entity;
                    if (defender == null)
                    {
                        continue;
                    }

                    MoveHelper.Move(defender, enemy.Position);
                    units.Remove(defender);
                    enemies.Remove(enemy);
                }
            }
        }
    }
}