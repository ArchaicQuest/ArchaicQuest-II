using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Character.Class;

namespace ArchaicQuestII.GameLogic.Character;

public class CharacterHandler : ICharacterHandler
{
    private ConcurrentDictionary<string, IClass> _classes;

    public CharacterHandler()
    {
        _classes = new ConcurrentDictionary<string, IClass>();

        var classTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IClass).IsAssignableFrom(p) && !p.IsInterface);

        foreach (var t in classTypes)
        {
            var c = (IClass)Activator.CreateInstance(t);
            if (c == null)
                continue;
            _classes.TryAdd(c.Name, c);
        }
        ;
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

    public List<IClass> GetClasses(bool includeSubs)
    {
        if (includeSubs)
            return _classes.Values.ToList();

        return _classes.Values.Where(x => x.IsSubClass == false).ToList();
    }

    public IClass GetClass(string className)
    {
        switch (className)
        {
            case "Fighter":
                return GetClass(ClassName.Fighter);
            case "Cleric":
                return GetClass(ClassName.Cleric);
            case "Mage":
                return GetClass(ClassName.Mage);
            case "Rogue":
                return GetClass(ClassName.Rogue);
            case "Scholar":
                return GetClass(ClassName.Scholar);
            default:
                return GetClass(ClassName.Fighter);
        }
    }
}
