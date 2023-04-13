using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Class;

namespace ArchaicQuestII.GameLogic.Character;

public interface ICharacterHandler
{
    IClass GetClass(ClassName className);
    IClass GetClass(SubClassName className);
    IClass GetClass(string className);
    List<IClass> GetClasses(bool includeSubs);
}
