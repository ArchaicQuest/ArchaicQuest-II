using System;
using System.Threading.Tasks;

namespace ArchaicQuestII.GameLogic.Core
{
    public interface ILoop
    {
        int TickDelay { get; }
        bool ConfigureAwait { get; }
        void PreTick();
        void Tick();
        void PostTick();

        async Task Loop()
        {
            await Task.Delay(5000);

            while (true)
            {
                await Task.Delay(TickDelay);

                PreTick();

                try
                {
                    Tick();
                }
                catch (Exception) { }

                PostTick();
            }
        }
    }
}
