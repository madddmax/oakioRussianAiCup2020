using System;
using System.Collections.Generic;
using System.Linq;
using aicup2020.Game.Ai;
using Aicup2020.Model;

namespace Aicup2020.Game
{
    public static class Bot
    {
        public static void Do()
        {
            List<Entity> enemies = World.All(e => !e.My && !e.Mineral);
            List<Entity> turrets = World.All(e => e.My && e.Type == EntityType.Turret);

            List<Entity> army = World.All(e => e.My && (e.Type == EntityType.RangedUnit ||
                                                        e.Type == EntityType.MeleeUnit));

            CombatBehavior.Combat(turrets, enemies);
            RangedMeleeDistanceBehavior.Apply(army);
            CombatBehavior.Combat(army, enemies);
            DefenceBehavior.Defence(army, enemies);
            GuardBehavior.Guard(army);
            RushBehavior.Rush(army, enemies);
            ExpansionBehavior.Expansion(army);


            // Base
            // 1. try build BB {if not exists}
            // 2. try build RB {if not exists} or build RANGED
            // 3. try build MB {if not exists} or build MELEE {p=0.2}
            // 4. try build HOUSE {if max population reached}
            // 5. try build BUILDER {if count <= 20}
            // 6. try build BUILDER {if count <= optBuilders and [BB, RB, MB] exists}

            List<Entity> builders = World.All(e => e.My && e.Type == EntityType.BuilderUnit);
            List<Entity> builderBases = World.All(u => u.My && u.Type == EntityType.BuilderBase);
            //List<Entity> meleeBases = World.All(u => u.My && u.Type == EntityType.MeleeBase);
            List<Entity> rangedBases = World.All(u => u.My && u.Type == EntityType.RangedBase);

            if (builderBases.Count == 0)
            {
                BuildBehavior.Build(builders, EntityType.BuilderBase);
            }

            if (rangedBases.Count == 0)
            {
                BuildBehavior.Build(builders, EntityType.RangedBase);
            }

            // if (meleeBases.Count == 0)
            // {
            //     BuildBehavior.Build(builders, EntityType.MeleeBase);
            // }

            if (World.Population + World.PopulationUse + 5 >= World.MaxPopulation + World.PopulationProvide)
            {
                BuildBehavior.Build(builders, EntityType.House);
            }

            int builderLimit = World
                .All(e => e.Mineral)
                .Sum(e => e.Health) / (5 * World.Info[EntityType.BuilderUnit].Cost);
            int optBuildersCount = Math.Min(60, builderLimit);

            bool baseReady = builderBases.Count > 0 && rangedBases.Count > 0; //&& meleeBases.Count > 0;
            if (builders.Count <= 20 || (baseReady && builders.Count <= optBuildersCount))
            {
                ProduceBehavior.Produce(builderBases);
            }

            if (World.All(e => e.My && e.Type == EntityType.Turret).Count <= 8 &&
                World.All(e => e.My && e.Type == EntityType.RangedUnit).Count >= 5)
            {
                BuildBehavior.Build(builders, EntityType.Turret);
            }

            ProduceBehavior.Produce(rangedBases);
            //ProduceBehavior.Produce(meleeBases, 0.3);

            CowardBehavior.Coward(builders);
            RepairBehavior.Repair(builders);
            MineBehavior.Mine(builders);
        }
    }
}