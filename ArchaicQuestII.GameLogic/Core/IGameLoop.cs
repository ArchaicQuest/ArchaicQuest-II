using System;
using System.Threading.Tasks;

namespace ArchaicQuestII.GameLogic.Core;

public interface IGameLoop
{
    int TickDelay { get; }
    ICoreHandler Handler { get; set; }
    bool Enabled { get; protected set; }

    public void Start()
    {
        Enabled = true;
        Task.Run(Tick);
    }

    public void Stop()
    {
        Enabled = false;
    }

    protected void Loop();

    private async Task Tick()
    {
        try
        {
            while (Enabled)
            {
                await Task.Delay(TickDelay);
                Loop();
            }
        }
        catch (Exception x)
        {
            Console.Write(x.Message);
        }
    }
}