using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.Character.Alignment
{
    public interface IAlignment
    {
        void CreateAlignment(Alignment option);
        Alignment GetAlignment(int id);
        List<Alignment> GetAlignments();
    }
}