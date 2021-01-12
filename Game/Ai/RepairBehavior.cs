using System.Collections.Generic;
using Aicup2020.Game;
using Entity = Aicup2020.Game.Entity;

namespace aicup2020.Game.Ai
{
    public static class RepairBehavior
    {
        public static void Repair(List<Entity> builders)
        {
            if (builders.Count == 0)
            {
                return;
            }

            List<Entity> facilities = World.All(e => e.My && e.Health < e.MaxHealth && e.Facility);
            if (facilities.Count == 0)
            {
                return; // OK
            }

            ContinueRepair(builders);
            GotoRepair(builders, facilities);
        }

        private static void ContinueRepair(List<Entity> builders)
        {
            foreach (Entity builder in builders.Clone())
            {
                if (TryGetClosestFacility(builder, out Entity facility))
                {
                    facility.RepairesCount++;
                    builder.Action = Actions.Repair(facility);
                    builders.Remove(builder);
                }
            }
        }

        private static void GotoRepair(List<Entity> builders, List<Entity> facilities)
        {
            foreach (Entity facility in facilities)
            {
                List<Point> repairPoints = Helper.GetFacilityTerritory(facility, p =>
                {
                    Tile tile = World.Get(p);
                    return tile.CanMoveHere && !tile.Attacked;
                });

                (Entity builder, Point repairPoint, int distance) = Helper.FindNearest(builders, repairPoints);
                if (builder == null)
                {
                    continue;
                }

                if (facility.RepairesCount > 0)
                {
                    int ticksToRepair = (facility.MaxHealth - facility.Health) / facility.RepairesCount;
                    if (ticksToRepair < 1.5 * distance)
                    {
                        return;
                    }
                }

                MoveHelper.Move(builder, repairPoint);
                builders.Remove(builder);
            }
        }

        private static bool TryGetClosestFacility(Entity builder, out  Entity facility)
        {
            Point position = builder.Position;

            if (position.X > 0 && TryGetClosestFacility(position.Left, out facility) ||
                position.Y > 0 && TryGetClosestFacility(position.Down, out facility) ||
                position.X + 1 < World.Size && TryGetClosestFacility(position.Right, out facility) ||
                position.Y + 1 < World.Size && TryGetClosestFacility(position.Up, out facility))
            {
                return true;
            }

            facility = null;
            return false;
        }

        private static bool TryGetClosestFacility(Point position, out Entity facility)
        {
            Entity entity = World.Get(position).Entity;
            if (entity == null)
            {
                facility = default;
                return false;
            }

            if (entity.My && entity.Facility && entity.Health < entity.MaxHealth)
            {
                facility = entity;
                return true;
            }

            facility = default;
            return false;
        }
    }
}