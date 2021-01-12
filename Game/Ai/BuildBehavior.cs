using System.Collections.Generic;
using System.Linq;
using Aicup2020.Game;
using Aicup2020.Model;
using Entity = Aicup2020.Game.Entity;

namespace aicup2020.Game.Ai
{
    public static class BuildBehavior
    {
        public static void Build(List<Entity> builders, EntityType type)
        {
            EntityInfo info = World.Info[type];

            if (World.Money < info.Cost)
            {
                return; // no money
            }

            int size = info.Size;
            Entity bestBuilder = null;
            Point bestSpawn = new Point();
            int minDistance = int.MaxValue;

            foreach (Entity builder in builders)
            {
                int x = builder.Position.X;
                int y = builder.Position.Y;

                for (int i = 0; i < size; i++)
                {
                    var left = new Point(x - size, y - i); // left side
                    if (CheckSpawn(type, left, size, ref minDistance, ref bestSpawn))
                    {
                        bestBuilder = builder;
                    }

                    var right = new Point(x + 1, y - i); // right side
                    if(CheckSpawn(type, right, size, ref minDistance, ref bestSpawn))
                    {
                        bestBuilder = builder;
                    }

                    var top = new Point(x - i, y + 1); // top side
                    if(CheckSpawn(type, top, size, ref minDistance, ref bestSpawn))
                    {
                        bestBuilder = builder;
                    }

                    var bottom = new Point(x - i, y - size); // bottom size
                    if(CheckSpawn(type, bottom, size, ref minDistance, ref bestSpawn))
                    {
                        bestBuilder = builder;
                    }
                }
            }

            if (bestBuilder == null)
            {
                return;
            }

            bestBuilder.Action = Actions.Build(type, bestSpawn);
            builders.Remove(bestBuilder);

            for (int x = bestSpawn.X; x < bestSpawn.X + size; x++)
            {
                for (int y = bestSpawn.Y; y < bestSpawn.Y + size; y++)
                {
                    World.Get(x, y).ReservedForEntity = bestBuilder;
                }
            }

            World.Money -= info.Cost;
            World.PopulationProvide += info.PopulationProvide;
            World.PopulationUse += info.PopulationUse;
        }

        private static bool CheckSpawn(EntityType type, Point candidate, int size, ref int minDistance, ref Point spawn)
        {
            if (!CanBuildHere(candidate, size))
            {
                return false;
            }

            if (type == EntityType.Turret)
            {
                if (candidate.X < 18 && candidate.Y < 18 ||
                    candidate.X < 8 || candidate.Y < 8)
                {
                    return false;
                }

                List<Entity> turrets = World.All(e => e.My && e.Type == EntityType.Turret);
                EntityTarget nearestTurret = Helper.GetNearest(candidate, turrets);
                if (nearestTurret != null && nearestTurret.Distance < 5)
                {
                    return false;
                }
            }

            if (type == EntityType.RangedBase)
            {
                if (candidate.X < 5 || candidate.Y < 5)
                {
                    return false;
                }
            }

            var target = new Point(0, 0);
            int distance = candidate.L1(target);
            if (distance < minDistance)
            {
                spawn = candidate;
                minDistance = distance;
                return true;
            }

            return false;
        }

        private static bool CanBuildHere(Point point, int size)
        {
            int x0 = point.X;
            int y0 = point.Y;

            if (x0 < 0 || x0 + size > World.Size ||
                y0 < 0 || y0 + size > World.Size)
            {
                return false; // out of range
            }

            for (int x = x0; x < x0 + size; x++)
            {
                for (int y = y0; y < y0 + size; y++)
                {
                    if (!World.Get(x, y).CanBuildHere)
                    {
                        return false;
                    }
                }
            }

            if (x0 == 0 ||
                y0 == 0 ||
                x0 + size == World.Size ||
                y0 + size == World.Size)
            {
                return true;
            }

            for (int i = 0; i < size + 2; i++)
            {
                var left = new Point(x0 - 1, y0 - 1 + i);
                if (IsFacility(left))
                {
                    return false;
                }

                var right = new Point(x0 + size, y0 - 1 + i);
                if (IsFacility(right))
                {
                    return false;
                }

                var top = new Point(x0 - 1 + i, y0 + size);
                if (IsFacility(top))
                {
                    return false;
                }

                var bottom = new Point(x0 - 1 + i, y0 - 1);
                if (IsFacility(bottom))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsFacility(Point point)
        {
            Entity entity = World.Get(point).Entity;
            return entity != null &&entity.Facility;
        }
    }
}