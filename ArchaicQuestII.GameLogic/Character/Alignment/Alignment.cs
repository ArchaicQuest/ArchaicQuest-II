using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Character.Alignment
{
    public class Alignment: Option, IAlignment
    {
        /// <summary>
        /// Value determines Alignment value
        ///   
        ///Evil    - Alignment between and including -1000 and -350
        ///Neutral - Alignment between and including -349 and 349
        ///Good    - Alignment between and including 350 and 1000
        /// </summary>
        public int Value { get; set; }

        private IDataBase _db { get; }

        public Alignment(IDataBase db)
        {
            _db = db;
        }

        public void CreateAlignment(Alignment option)
        {
            _db.Save(option, "Alignment");

            // TODO: check
            //col.Insert(option);
            //col.EnsureIndex(x => x.Name);
        }

        public Alignment GetAlignment(int id)
        {
            return _db.GetById<Alignment>(id, "Alignment");
        }

        public List<Alignment> GetAlignments()
        {
            return _db.GetCollection<Alignment>("Alignment").FindAll().OrderBy(x => x.Name).ToList();
        }
    }
}
