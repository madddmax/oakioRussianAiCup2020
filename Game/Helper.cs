using System;
using System.Collections.Generic;
using System.Linq;
using Aicup2020.Model;

namespace Aicup2020.Game
{
    public static class Helper
    {
        public static (Entity closest, int distance) GetNearestL1(Point point, ICollection<Entity> entities)
        {
            int minDistance = Int32.MaxValue;
            Entity closest = null;

            foreach (Entity entity in entities)
            {
                int distance = point.L1(entity.Position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = entity;
                }
            }

            return (closest, minDistance);
        }

        public static PointTarget GetNearest(Point point, ICollection<Point> others)
        {
            PointTarget target = null;

            foreach (Point other in others)
            {
                int distance = point.L1(other);
                if (target == null || distance < target.Distance)
                {
                    target = new PointTarget(other, distance);
                }
            }

            return target;
        }

        public static (Entity closest, Point p, int distance) FindNearest(List<Entity> entities, List<Point> positions)
        {
            Entity gClosest = null;
            Point gP = new Point();
            int gDist = int.MaxValue;

            foreach (Point position in positions)
            {
                (Entity closest, int distance) = GetNearestL1(position, entities);
                if (closest != null && distance < gDist)
                {
                    gClosest = closest;
                    gP = position;
                    gDist = distance;
                }
            }
            return (gClosest, gP, gDist);
        }

        public static EntityTarget GetNearest(Point point, ICollection<Entity> entities)
        {
            EntityTarget target = null;

            foreach (Entity entity in entities)
            {
                int distance = point.L1(entity.Position);
                if (target == null || distance < target.Distance)
                {
                    target = new EntityTarget(entity, distance);
                }
            }

            return target;
        }

        public static List<EntityTarget> GetNearestInRange(Entity entity, List<Entity> others, int range)
        {
            Point position = entity.Position;
            int size = entity.Size;

            var targets = new List<EntityTarget>();
            foreach (Entity other in others)
            {
                if (other.Id == entity.Id)
                {
                    continue;
                }

                if (size > 1)
                {
                    int minDistance = int.MaxValue;
                    for (int i = 0; i < size; i++)
                    {
                        var left = new Point(position.X, position.Y + i);
                        int distance = left.L1(other.Position);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                        }

                        var right = new Point(position.X + size - 1, position.Y + i);
                        distance = right.L1(other.Position);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                        }

                        var top = new Point(position.X + i, position.Y + size - 1);
                        distance = top.L1(other.Position);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                        }

                        var bottom = new Point(position.X + i, position.Y);
                        distance = bottom.L1(other.Position);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                        }
                    }

                    if (minDistance <= range)
                    {
                        targets.Add(new EntityTarget(other, minDistance));
                    }
                }
                else
                {
                    int distance = position.L1(other.Position);
                    if (distance <= range)
                    {
                        targets.Add(new EntityTarget(other, distance));
                    }
                }
            }

            targets.Sort();
            return targets;
        }

        public static List<T> Clone<T>(this List<T> self) => new List<T>(self);

        public static List<Point> GetFacilityTerritory(Entity facility, Func<Point, bool> condition)
        {
            // территория вокруг здания без учета углов
            int x = facility.Position.X;
            int y = facility.Position.Y;
            int size = facility.Size;

            var result = new List<Point>(4 * size);
            for (int i = 0; i < size; i++)
            {
                var left = new Point(x - 1, y + i);
                if (x > 0 && condition(left)) // left line
                {
                    result.Add(left);
                }

                var right = new Point(x + size, y + i);
                if (x + size < World.Size && condition(right)) // right line
                {
                    result.Add(right);
                }

                var bottom = new Point(x + i, y - 1);
                if (y > 0 && condition(bottom)) // bottom line
                {
                    result.Add(bottom);
                }

                var top = new Point(x + i, y + size);
                if (y + size < World.Size && condition(top)) // top line
                {
                    result.Add(top);
                }
            }

            return result;
        }

        public static IEnumerable<Point> GetNeighbors(Point position)
        {
            if (position.X > 0)
            {
                yield return position.Left;
            }

            if (position.Y > 0)
            {
                yield return position.Down;
            }

            if (position.X + 1 < World.Size)
            {
                yield return position.Right;
            }

            if (position.Y + 1 < World.Size)
            {
                yield return position.Up;
            }
        }
    }

    public class EntityTarget : IComparable<EntityTarget>
    {
        public readonly Entity Entity;
        public readonly int Distance;

        public EntityTarget(Entity entity, int distance)
        {
            Entity = entity;
            Distance = distance;
        }

        public override string ToString() => $"{Entity} distance {Distance}";

        public int CompareTo(EntityTarget other) => Distance.CompareTo(other.Distance);
    }

    public class PointTarget : IComparable<PointTarget>
    {
        public readonly Point Target;
        public readonly int Distance;

        public PointTarget(Point target, int distance)
        {
            Target = target;
            Distance = distance;
        }

        public int CompareTo(PointTarget other) => Distance.CompareTo(other.Distance);
    }
}