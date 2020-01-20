using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.Engine.Character.Race
{
    interface IRace
    {
        void Create(Model.Race race);

        Model.Race Get(int id);

        List<Model.Race> GetAll();
    }
}
