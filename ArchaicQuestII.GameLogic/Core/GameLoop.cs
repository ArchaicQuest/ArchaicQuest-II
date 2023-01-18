using System;
using System.Threading.Tasks;

namespace ArchaicQuestII.GameLogic.Core;

public abstract class GameLoop
{
    public string Name = "";
    private const int TickDelay = 60000;
    protected readonly ICoreHandler Handler;
    private bool _enabled;

    protected GameLoop(ICoreHandler handler)
    {
        Handler = handler;
    }

    public void Start()
    {
        _enabled = true;
        Task.Run(Tick);
    }

    public void Stop()
    {
        _enabled = false;
    }

    protected virtual void Event() {}

    private async Task Tick()
    {
        try
        {
            while (_enabled)
            {
                await Task.Delay(TickDelay);
                Event();
            }
        }
        catch (Exception x)
        {
            Console.Write(x);
        }
    }
}