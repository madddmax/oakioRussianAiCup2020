using Aicup2020.Model;

namespace Aicup2020.Game
{
    public class EntityInfo
    {
        public int Cost;
        public readonly int Size;
        public readonly int PopulationProvide;
        public readonly int PopulationUse;

        public EntityInfo(EntityProperties p)
        {
            Cost = p.InitialCost;
            Size = p.Size;
            PopulationProvide = p.PopulationProvide;
            PopulationUse = p.PopulationUse;
        }
    }
}