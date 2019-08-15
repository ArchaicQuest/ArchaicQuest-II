using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Core.Events;
using LiteDB;

namespace ArchaicQuestII.Engine.Character.Race.Commands
{
    public class CreateRaceCommand
    {
        private static Log.Log Logger { get; set; }

        public CreateRaceCommand()
        {
            Logger = new Log.Log();
        }

        public void CreateRace(Model.Race race)
        {
            if (race == null)
            {
                Logger.Error("Can't save race as race is null");
                throw new ArgumentNullException(nameof(race));
            }

            try
            {
                using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
                {
                    var col = db.GetCollection<Model.Race>("Race");

                    col.Insert(race);
                    col.EnsureIndex(x => x.Name);

                }

            }
            catch (Exception ex)
            {
                Logger.Error("Error Saving race " + ex.Message);
            }
        }
    }
}
