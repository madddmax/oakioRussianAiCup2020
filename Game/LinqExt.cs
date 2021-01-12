using System.Collections.Generic;
using System.Linq;

namespace Aicup2020.Game
{
    public static class LinqExt
    {
        public static IEnumerable<Entity> AsEntities(this IEnumerable<Point> points)
        {
            var entities = new HashSet<int>();
            foreach (Point point in points)
            {
                Tile tile = World.Get(point);
                Entity entity = tile.Entity;

                if (entity == null || entities.Contains(entity.Id))
                {
                    continue;
                }

                entities.Add(entity.Id);
                yield return entity;
            }
        }

        public static IEnumerable<Tile> AsTiles(this IEnumerable<Point> points) => points.Select(World.Get);

        public static IEnumerable<Point> AsPoints(this IEnumerable<Tile> tiles) => tiles.Select(tile => tile.Position);
    }
}