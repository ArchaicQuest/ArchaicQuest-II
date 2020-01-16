using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.Engine.Core.Interface;
using LiteDB;

namespace ArchaicQuestII.Engine.Character.Class.Commands
{
    public class CreateAttackTypeCommand
    {

        private IDataBase _db { get; }
        public CreateAttackTypeCommand(IDataBase db)
        {
            _db = db;
        }
 
        public void CreateAttackType(Option option)
        {

            _db.Save(option, "AttackType");
             
        }
    }
}
