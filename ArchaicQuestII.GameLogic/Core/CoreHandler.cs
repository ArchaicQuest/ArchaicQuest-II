using System;
using System.Collections.Concurrent;
using System.Linq;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World;
using Microsoft.Extensions.DependencyInjection;

namespace ArchaicQuestII.GameLogic.Core;

public class CoreHandler : ICoreHandler
{
    public Config Config { get; set; }
    public ICharacterHandler Character { get; }
    public IClientHandler Client { get; }
    public ICombatHandler Combat { get; }
    public ICommandHandler Command { get; }
    public IItemHandler Item { get; }
    public IWorldHandler World { get; }
    public IDataBase Db { get; }
    public IPlayerDataBase Pdb { get; }

    private ConcurrentDictionary<string, GameLoop> _gameLoops = new();

    private bool _loopsStarted = false;

    public CoreHandler(IServiceProvider serviceProvider)
    {
        Config = new Config();
        Character = serviceProvider.GetService<ICharacterHandler>();
        Client = serviceProvider.GetService<IClientHandler>();
        Combat = serviceProvider.GetService<ICombatHandler>();
        Command = serviceProvider.GetService<ICommandHandler>();
        Item = serviceProvider.GetService<IItemHandler>();
        World = serviceProvider.GetService<IWorldHandler>();
        Db = serviceProvider.GetService<IDataBase>();
        Pdb = serviceProvider.GetService<IPlayerDataBase>();
        
        SetupLoops();
    }

    private void SetupLoops()
    {
        var gameLoops = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(GameLoop).IsAssignableFrom(p) && !p.IsAbstract);

        foreach (var t in gameLoops)
        {
            var loop = (GameLoop)Activator.CreateInstance(t, this);
            
            if(loop != null)
                _gameLoops.TryAdd(loop.GetType().Name, loop);
        }
    }
    
    public void StartAllLoops()
    {
        if (_loopsStarted) return;

        foreach (var loop in _gameLoops.Values)
        {
            loop.Start();
        }

        _loopsStarted = true;
    }
    
    public void StopAllLoops()
    {
        if (!_loopsStarted) return;

        foreach (var loop in _gameLoops.Values)
        {
            loop.Stop();
        }

        _loopsStarted = false;
    }
}