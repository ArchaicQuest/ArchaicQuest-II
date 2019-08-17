using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Item;
using System.Data;
using ArchaicQuestII.Engine.Core.Events;
using ArchaicQuestII.Engine.Effect;
using ArchaicQuestII.Engine.Skill;
using ArchaicQuestII.Engine.World.Room.Model;

namespace ArchaicQuestII.Engine.Spell
{
    public interface ISpells
    {
        void DoSpell(Model.Spell spell, Player origin, Player target, Room room = null);


    }


}

