using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.World.Room;
using Moq;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Spell.Interface;

namespace ArchaicQuestII.GameLogic.Tests
{
    public class SpellTests
    {
        private readonly Spell.Model.Spell _Spells;
        private readonly Player _player;
        private readonly Player _target;
        private readonly Room _room;
        private readonly CastSpell _spell;
        private readonly Mock<IWriteToClient> _writer;
        private readonly Mock<IDamage> _damage;
        private readonly Mock<Cache> _cache;
        private readonly Mock<ISpellTargetCharacter> _spellTargetCharacter;
        private readonly Mock<IUpdateClientUI> _updateClientUI;
        private readonly Mock<IMobScripts> _mobScript;
        private readonly Mock<ISpellList> _spellList;
    }
}
