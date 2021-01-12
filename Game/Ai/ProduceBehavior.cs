using System;
using System.Collections.Generic;
using Aicup2020.Game;
using Aicup2020.Model;
using Entity = Aicup2020.Game.Entity;

namespace aicup2020.Game.Ai
{
    public static class ProduceBehavior
    {
        public static void Produce(List<Entity> factories)
        {
            foreach (Entity factory in factories)
            {
                ProduceUnit(factory);
            }
        }

        private static void ProduceUnit(Entity factory)
        {
            if (!factory.Active)
            {
                return; // not ready
            }

            EntityType unitType = GetProduceEntityType(factory.Type);
            EntityInfo info = World.Info[unitType];

            if (World.Money < info.Cost)
            {
                return; // no money
            }

            if (World.Population + World.PopulationUse == World.MaxPopulation)
            {
                return; // no living space
            }

            List<Point> spawns = Helper.GetFacilityTerritory(factory, IsFree);
            if (spawns.Count == 0)
            {
                return;
            }

            Point spawn = Dice.Roll(spawns);
            factory.Action = Actions.Build(unitType, spawn);
            World.Get(spawn).ReservedForEntity = factory;

            World.Money -= info.Cost;
            World.PopulationUse += info.PopulationUse;
            World.PopulationProvide += info.PopulationProvide;
        }

        private static bool IsFree(Point position) => World.Get(position).CanSpawnHere;

        private static EntityType GetProduceEntityType(EntityType factory)
        {
            switch (factory)
            {
                case EntityType.BuilderBase: return EntityType.BuilderUnit;
                case EntityType.MeleeBase: return EntityType.MeleeUnit;
                case EntityType.RangedBase: return EntityType.RangedUnit;
                default: throw new InvalidOperationException($"Unknown factory {factory}");
            }
        }
    }
}