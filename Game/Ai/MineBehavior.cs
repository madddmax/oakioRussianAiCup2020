using System.Collections.Generic;
using System.Linq;
using Aicup2020.Game;
using Aicup2020.Model;
using Entity = Aicup2020.Game.Entity;

namespace aicup2020.Game.Ai
{
    public static class MineBehavior
    {
        public static void Mine(List<Entity> builders)
        {
            MineClosestMineral(builders);
            GotoMinerals(builders);
        }

        private static void MineClosestMineral(List<Entity> builders)
        {
            foreach (Entity builder in builders.Clone())
            {
                Point position = builder.Position;
                List<Entity> resources = Helper
                    .GetNeighbors(position)
                    .AsEntities()
                    .Where(e => e.Mineral)
                    .ToList();

                if (resources.Count == 0)
                {
                    continue;
                }

                Entity mineral = Dice.Roll(resources);
                builder.Action = Actions.Attack(mineral);
                builders.Remove(builder);
            }
        }

        private static void GotoMinerals(List<Entity> builders)
        {
            HashSet<Point> positions = GetMiningPositions();

            foreach (Entity builder in builders)
            {
                PointTarget target = Helper.GetNearest(builder.Position, positions);
                if (target == null)
                {
                    continue;
                }

                Point targetPosition = target.Target;
                positions.Remove(targetPosition);
                MoveHelper.Move(builder, targetPosition);
            }
        }

        private static HashSet<Point> GetMiningPositions()
        {
            List<Entity> minerals = World.All(e => e.Type == EntityType.Resource);
            var positions = new HashSet<Point>();

            foreach (Entity mineral in minerals)
            {
                Point position = mineral.Position;
                IsFreeAndSafe(positions, position.Left);
                IsFreeAndSafe(positions, position.Right);
                IsFreeAndSafe(positions, position.Up);
                IsFreeAndSafe(positions, position.Down);
            }

            return positions;
        }

        private static void IsFreeAndSafe(HashSet<Point> positions, Point position)
        {
            if (World.TryGet(position, out Tile tile) && tile.CanMoveHere && !tile.Attacked)
            {
                positions.Add(position);
            }
        }
    }
}