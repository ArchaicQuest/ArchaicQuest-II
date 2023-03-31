using System;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Commands;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.World.Room;
using Newtonsoft.Json;
using System.Web;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Utilities;

namespace ArchaicQuestII.GameLogic.Core
{
    public class GameLoop : IGameLoop
    {
        private ICore _core;
        private ICommandHandler _commandHandler;
        private List<string> _hints;
        private List<ILoop> _loops;

        public GameLoop(ICore core, ICommandHandler commandHandler)
        {
            _core = core;
            _commandHandler = commandHandler;

            var loopTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ILoop).IsAssignableFrom(p) && !p.IsInterface);

            foreach (var t in loopTypes)
            {
                var loop = (ILoop)Activator.CreateInstance(t);
                if (loop == null) continue;

                loop.Init(_core, _commandHandler);
                _loops.Add(loop);
            }
        }

        public void StartLoops()
        {
            foreach(var loop in _loops)
            {
                Task.Run(loop.Loop).ConfigureAwait(loop.ConfigureAwait);
            }
        }
    }
}