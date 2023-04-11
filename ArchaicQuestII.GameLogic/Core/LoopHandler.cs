using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.GameLogic.Core
{
    public class LoopHandler : ILoopHandler
    {
        private List<string> _hints;
        private List<ILoop> _loops;

        public LoopHandler()
        {
            _loops = new List<ILoop>();

            var loopTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ILoop).IsAssignableFrom(p) && !p.IsInterface);

            foreach (var t in loopTypes)
            {
                var loop = (ILoop)Activator.CreateInstance(t);
                if (loop == null)
                    continue;

                _loops.Add(loop);
            }
        }

        public void StartLoops()
        {
            foreach (var loop in _loops)
            {
                Task.Run(loop.Loop).ConfigureAwait(loop.ConfigureAwait);
            }
        }
    }
}
