using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Skill.Skills;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.Commands;

namespace ArchaicQuestII.GameLogic.Core
{
    public sealed class Services
    {
        public ICache Cache { get; private set; }
        public IWriteToClient Writer { get; private set; }
        public IDataBase DataBase { get; private set; }
        public IPlayerDataBase PlayerDataBase { get; private set; }
        public IUpdateClientUI UpdateClient { get; private set; }
        public ICombat Combat { get; private set; }
        public IRoomActions RoomActions { get; private set; }
        public IAreaActions AreaActions { get; private set; }
        public IMobScripts MobScripts { get; private set; }
        public IPassiveSkills PassiveSkills { get; private set; }
        public IFormulas Formulas { get; private set; }
        public IErrorLog ErrorLog { get; private set; }
        public ITime Time { get; private set; }
        public IDamage Damage { get; private set; }
        public ISpellList SpellList { get; private set; }
        public IWeather Weather { get; private set; }
        public ICharacterHandler CharacterHandler { get; private set; }
        public ILoopHandler GameLoop { get; private set; }
        public ICommandHandler CommandHandler { get; private set; }

        private static readonly Services instance = new Services();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Services() { }

        public static Services Instance
        {
            get { return instance; }
        }

        private Services()
        {
            Cache = new Cache();
        }

        public void InitServices(
            IWriteToClient writeToClient,
            IDataBase dataBase,
            IUpdateClientUI updateClient,
            ICombat combat,
            IPlayerDataBase playerDataBase,
            IRoomActions roomActions,
            IMobScripts mobScripts,
            IErrorLog errorLog,
            IPassiveSkills passiveSkills,
            IFormulas formulas,
            ITime time,
            IDamage damage,
            ISpellList spellList,
            IWeather weather,
            ICharacterHandler characterHandler,
            ILoopHandler gameLoop,
            ICommandHandler commandHandler
        )
        {
            Writer = writeToClient;
            DataBase = dataBase;
            UpdateClient = updateClient;
            Combat = combat;
            PlayerDataBase = playerDataBase;
            RoomActions = roomActions;
            MobScripts = mobScripts;
            ErrorLog = errorLog;
            PassiveSkills = passiveSkills;
            Formulas = formulas;
            Time = time;
            Damage = damage;
            SpellList = spellList;
            Weather = weather;
            CharacterHandler = characterHandler;
            GameLoop = gameLoop;
            CommandHandler = commandHandler;
        }
    }
}
