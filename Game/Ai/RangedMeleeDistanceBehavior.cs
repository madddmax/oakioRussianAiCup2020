using System.Collections.Generic;
using System.Linq;
using Aicup2020.Game;
using Aicup2020.Model;
using Entity = Aicup2020.Game.Entity;

namespace aicup2020.Game.Ai
{
    public static class RangedMeleeDistanceBehavior
    {
        public static void Apply(List<Entity> units)
        {
            List<Entity> ranges = units.Where(u => u.Type == EntityType.RangedUnit).ToList();

            List<Entity> enemies = World.All(e => !e.My && e.Type == EntityType.MeleeUnit);

            foreach (Entity ranged in ranges)
            {
                EntityTarget target = Helper.GetNearest(ranged.Position, enemies);
                if (target == null)
                {
                    continue;
                }

                if (target.Distance == ranged.AttackDistance + target.Entity.AttackDistance)
                {
                    units.Remove(ranged);
                }
                else
                {
                    ;
                    if (target.Distance == 2)
                    {
                        var cowardPositions = Helper
                            .GetNeighbors(ranged.Position)
                            .AsTiles()
                            .Where(t => t.CanMoveHere && !t.Attacked)
                            .AsPoints()
                            .ToList();

                        if (cowardPositions.Count > 0) // Не убегать если FATALITY!
                        {
                            var p = Dice.Roll(cowardPositions);
                            MoveHelper.Move(ranged, p);
                            units.Remove(ranged);
                        }
                    }
                }
            }
        }
    }
}