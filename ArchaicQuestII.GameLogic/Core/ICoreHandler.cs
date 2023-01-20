using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World;
using Microsoft.AspNetCore.Builder;

namespace ArchaicQuestII.GameLogic.Core;

public interface ICoreHandler
{
    Config Config { get; set; }
    ICharacterHandler Character { get; }
    IClientHandler Client { get; }
    ICombatHandler Combat { get; }
    ICommandHandler Command { get; }
    IItemHandler Item { get; }
    IWorldHandler World { get; }
    IDataBase Db { get; }
    public IPlayerDataBase Pdb { get; }
    public void Init(IApplicationBuilder app);
    void StartAllLoops();
    void StopAllLoops();
}