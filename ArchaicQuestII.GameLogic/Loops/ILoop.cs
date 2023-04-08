using System;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Commands;

namespace ArchaicQuestII.GameLogic.Core
{
	public interface ILoop
	{
		int TickDelay { get; }
        bool ConfigureAwait { get; }
        void Init(ICore core, ICommandHandler commandHandler);
		void PreTick();
		void Tick();
		void PostTick();

		async Task Loop()
		{
            while (true)
            {
                await Task.Delay(TickDelay);

                PreTick();

                try
                {
                    Tick();
                }
                catch (Exception)
                {

                }

                PostTick();
            }
        }
	}
}

