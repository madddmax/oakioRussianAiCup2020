using System;
using System.Collections.Generic;
using System.Linq;
using Aicup2020.Model;

namespace Aicup2020.Game
{
    public static class World
    {
        private static readonly List<Entity> Entities = new List<Entity>();
        private static Tile[,] _map;

        public static int Size;

        public static int Money;
        public static int Population;
        public static int PopulationUse;

        public static int MaxPopulation;
        public static int PopulationProvide;

        public static readonly Dictionary<EntityType, EntityInfo> Info = new Dictionary<EntityType, EntityInfo>();

        //public static bool HasMoney(EntityType type) => Costs[type] <= Money;

        public static List<Entity> All(Func<Entity, bool> selector) => Entities.Where(selector).ToList();

        public static int Count(Func<Entity, bool> selector) => Entities.Count(selector);

        public static void Update(PlayerView playerView)
        {
            Size = playerView.MapSize;
            if (_map == null)
            {
                _map = new Tile[Size, Size];
            }

            Entities.Clear();
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    _map[i, j] = new Tile(new Point(i, j));
                }
            }

            foreach (Player player in playerView.Players)
            {
                if (player.Id == playerView.MyId)
                {
                    Money = player.Resource;
                }
            }

            Info.Clear();
            IDictionary<EntityType,EntityProperties> allProps = playerView.EntityProperties;
            foreach ((EntityType type, EntityProperties props) in playerView.EntityProperties)
            {
                Info.Add(type, new EntityInfo(props));
            }

            Population = 0;
            PopulationUse = 0;
            MaxPopulation = 0;
            PopulationProvide = 0;

            foreach (Model.Entity model in playerView.Entities)
            {
                EntityProperties properties = allProps[model.EntityType];
                int size = properties.Size;

                var entity = new Entity(
                    model,
                    properties,
                    model.PlayerId == playerView.MyId);

                Entities.Add(entity);

                if (size > 1)
                {
                    for (int cx = 0; cx < size; cx++)
                    {
                        for (int cy = 0; cy < size; cy++)
                        {
                            Tile tile = _map[entity.Position.X + cx, entity.Position.Y + cy];
                            tile.Entity = entity;
                        }
                    }
                }
                else
                {
                    Tile tile = _map[entity.Position.X, entity.Position.Y];
                    tile.Entity = entity;
                }

                if (entity.My)
                {
                    Population += properties.PopulationUse;
                    MaxPopulation += properties.PopulationProvide;

                    if (model.EntityType == EntityType.BuilderUnit ||
                        model.EntityType == EntityType.MeleeUnit ||
                        model.EntityType == EntityType.RangedUnit)
                    {
                        Info[model.EntityType].Cost++;
                    }
                }
                else
                {
                    UpdateAttackedMap(entity);
                }
            }
        }

        private static void UpdateAttackedMap(Entity entity)
        {
            Point position = entity.Position;

            const int forecast = 1;
            switch (entity.Type)
            {
                case EntityType.MeleeUnit:
                {
                    SetAttacked(position, 1 + forecast);
                    return;
                }
                case EntityType.RangedUnit:
                {
                    SetAttacked(position, 5 + forecast);
                    break;
                }
                case EntityType.Turret:
                {
                    //TODO
                    break;
                }
            }
        }

        private static void SetAttacked(Point center, int distance)
        {
            int x0 = center.X;
            int y0 = center.Y;

            for (int x = 0; x <= distance; x++)
            {
                for (int y = 0; y <= distance - x; y++)
                {
                    if (x0 + x < Size)
                    {
                        if (y0 + y < Size)
                        {
                            _map[x0 + x, y0 + y].Attacked = true;
                        }

                        if (y0 - y >= 0)
                        {
                            _map[x0 + x, y0 - y].Attacked = true;
                        }
                    }

                    if (x0 - x >= 0)
                    {
                        if (y0 + y < Size)
                        {
                            _map[x0 - x, y0 + y].Attacked = true;
                        }

                        if (y0 - y >= 0)
                        {
                            _map[x0 - x, y0 - y].Attacked = true;
                        }
                    }
                }
            }
        }

        public static Tile Get(Point point) => Get(point.X, point.Y);

        public static Tile Get(int x, int y) => _map[x, y];

        public static bool TryGet(Point point, out Tile tile)
        {
            if (Inside(point))
            {
                tile = Get(point);
                return true;
            }

            tile = default;
            return false;
        }

        private static bool Inside(Point point) =>
            point.X >= 0 && point.X < Size &&
            point.Y >= 0 && point.Y < Size;
    }
}