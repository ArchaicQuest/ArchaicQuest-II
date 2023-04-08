namespace ArchaicQuestII.GameLogic.Character;

public enum ClassName {
    None,
    Fighter = 1,
    Rogue = 2,
    Mage = 3,
    Cleric = 4,
    Scholar = 5,
}

public enum SubClassName {
    None = 0,

    //Fighter Subs
    Ranger = 1,
    Barbarian = 2,
    Swashbuckler = 3,
    Armsman = 4,
    Samurai = 5,

    //Rogue Subs
    Assassin = 21,
    Bandit = 22,
    Nightshade = 23,
    Pirate = 24,
    Ninja = 25,

    //Mage Subs
    Witch = 31,
    Illusionist = 32, 
    Enchanter = 33,
    Mentalist = 34,
    Conjuror = 35,
    Invoker = 36,

    //Cleric Subs
    Crusader = 41, 
    Druid = 42,
    Shaman = 43,
    Priest = 44,

    //Scholar Subs
    Alchemist = 51,
    Engineer = 52,
    Herbalist = 53,
    Poisoner = 54
}