using System.Collections.Generic;
using System.Linq;
using Aicup2020.Game;

namespace aicup2020.Game.Ai
{
    public static class CowardBehavior
    {
        public static void Coward(List<Entity> builders)
        {
            List<Entity> defenders = World.All(u => u.My && u.CanAttack);

            foreach (Entity builder in builders.ToList())
            {
                Point position = builder.Position;

                if (!World.Get(position).Attacked)
                {
                    continue;
                }

                var safePositions = new List<Point>(4);
                TryAdd(safePositions, position.Left);
                TryAdd(safePositions, position.Right);
                TryAdd(safePositions, position.Up);
                TryAdd(safePositions, position.Down);

                if (safePositions.Count == 0)
                {
                    return; // fatal, no way
                }

                Point safePosition = SelectSafePosition(safePositions, defenders);
                MoveHelper.Move(builder, safePosition);
                builders.Remove(builder);
            }
        }

        private static Point SelectSafePosition(List<Point> safePositions, List<Entity> defenders)
        {
            if (defenders.Count == 0)
            {
                return Dice.Roll(safePositions);
            }

            (Entity defender, Point safePoint, int _) = Helper.FindNearest(defenders, safePositions);
            //defenders.Remove(defender);
            return safePoint;
        }

        private static void TryAdd(List<Point> positions, Point position)
        {
            if (World.TryGet(position, out Tile tile) &&
                tile.CanMoveHere &&
                !tile.Attacked &&
                IsNotCorner(position)
                )
            {
                positions.Add(position);
            }
        }

        private static bool IsNotCorner(Point position) =>
            Helper
                .GetNeighbors(position)
                .AsTiles()
                .Any(t => t.CanMoveHere && !t.Attacked);
    }
}