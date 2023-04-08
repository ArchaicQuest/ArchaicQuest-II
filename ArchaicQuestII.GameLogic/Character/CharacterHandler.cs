using System;
using System.Collections.Concurrent;
using System.Linq;
using ArchaicQuestII.GameLogic.Character.Class;

namespace ArchaicQuestII.GameLogic.Character;

public sealed class CharacterHandler
{   
    private static readonly CharacterHandler instance = new CharacterHandler();

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static CharacterHandler()
    {

    }

    public static CharacterHandler Instance
    {
        get
        {
            return instance;
        }
    }

    private ConcurrentDictionary<string, IClass> _classes;

    private CharacterHandler()
    {
        _classes = new ConcurrentDictionary<string, IClass>();

        var classTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IClass).IsAssignableFrom(p) && !p.IsInterface);

        foreach (var t in classTypes)
        {
            var c = (IClass)Activator.CreateInstance(t);
            if (c == null) continue;
            _classes.TryAdd(c.Name, c);
        };
    }

    public IClass GetClass(ClassName className)
    {
        _classes.TryGetValue(className.ToString(), out var c);

        return c;
    }

    public IClass GetClass(SubClassName className)
    {
        _classes.TryGetValue(className.ToString(), out var c);

        return c;
    }
}