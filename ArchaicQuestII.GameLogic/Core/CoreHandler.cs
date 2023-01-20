using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ArchaicQuestII.GameLogic.Core;

public class CoreHandler : ICoreHandler
{
    public Config Config { get; set; }
    public ICharacterHandler Character { get; private set; }
    public IClientHandler Client { get; private set; }
    public ICombatHandler Combat { get; private set; }
    public ICommandHandler Command { get; private set; }
    public IItemHandler Item { get; private set; }
    public IWorldHandler World { get; private set; }
    public IDataBase Db { get; private set; }
    public IPlayerDataBase Pdb { get; private set; }

    private readonly ConcurrentDictionary<string, IGameLoop> _gameLoops = new();

    private bool _loopsStarted;

    public void Init(IApplicationBuilder app)
    {
        Config = new Config();
        Character = app.ApplicationServices.GetService<ICharacterHandler>();
        Client = app.ApplicationServices.GetService<IClientHandler>();
        Combat = app.ApplicationServices.GetService<ICombatHandler>();
        Command = app.ApplicationServices.GetService<ICommandHandler>();
        Item = app.ApplicationServices.GetService<IItemHandler>();
        World = app.ApplicationServices.GetService<IWorldHandler>();
        Db = app.ApplicationServices.GetService<IDataBase>();
        Pdb = app.ApplicationServices.GetService<IPlayerDataBase>();
        
        SetupLoops();
    }

    private void SetupLoops()
    {
        var gameLoops = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IGameLoop).IsAssignableFrom(p) && !p.IsInterface);

        foreach (var t in gameLoops)
        {
            var loop = (IGameLoop)Activator.CreateInstance(t);

            if (loop == null) continue;
            
            loop.Handler = this;
            _gameLoops.TryAdd(loop.GetType().Name, loop);
        }
    }
    
    public void StartAllLoops()
    {
        if (_loopsStarted) return;

        foreach (var loop in _gameLoops.Values)
        {
            loop.Start();
            Console.WriteLine($"{loop.GetType().Name} loop started.");
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