using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.Character.Alignment
{
    public interface IAlignment
    {
        void CreateAlignment(IAlignment option);
        Alignment GetAlignment(int id);
        List<Alignment> GetAlignments();
        void Seed();
        List<Alignment> SeedData()
    }
}