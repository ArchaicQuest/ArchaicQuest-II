using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;

namespace ArchaicQuestII.GameLogic.Character.Loops;

public class UpdatePlayersStats : IGameLoop
{
    public int TickDelay => 120000;
    public ICoreHandler Handler { get; set; }
    public bool Enabled { get; set; }

    public void Loop()
    {
        var players = Handler.Character.GetPlayerCache().Values.ToList();
        
        foreach (var player in players)
        {

            //  IdleCheck(player);

            var hP = DiceBag.Roll(1, 2, 5);
            var mana = DiceBag.Roll(1, 2, 5);
            var moves = DiceBag.Roll(1, 2, 5);

            // if player has fast healing add the bonus here
            var hasFastHealing = player.Skills.FirstOrDefault(x =>
                x.SkillName.Equals("Fast Healing", StringComparison.CurrentCultureIgnoreCase) &&
                player.Level >= x.Level);

            
            if ((player.Status & CharacterStatus.Status.Sleeping) != 0)
            {
                hP *= 2;
                mana *= 2;
                moves *= 2;
            }

            if ((player.Status & CharacterStatus.Status.Resting) != 0)
            {
                hP *= (int)1.5;
                mana *= (int)1.5;
                moves *= (int)1.5;
            }

            if (player.Attributes.Attribute[EffectLocation.Hitpoints] <
                player.MaxAttributes.Attribute[EffectLocation.Hitpoints])
            {

                if (hasFastHealing != null)
                {
                    if (Helpers.SkillSuccessCheck(hasFastHealing))
                    {
                        hP *= 2;
                    }
                    else
                    {
                        Handler.Character.GainSkillProficiency(hasFastHealing, player);
                    }
                }

                player.Attributes.Attribute[EffectLocation.Hitpoints] += Formulas.GainAmount(hP, player);
                if (player.Attributes.Attribute[EffectLocation.Hitpoints] >
                    player.MaxAttributes.Attribute[EffectLocation.Hitpoints])
                {
                    player.Attributes.Attribute[EffectLocation.Hitpoints] =
                        player.MaxAttributes.Attribute[EffectLocation.Hitpoints];
                }
            }

            if (player.Attributes.Attribute[EffectLocation.Mana] <
                player.MaxAttributes.Attribute[EffectLocation.Mana])
            {
                player.Attributes.Attribute[EffectLocation.Mana] += Formulas.GainAmount(mana, player);

                if (player.Attributes.Attribute[EffectLocation.Mana] >
                    player.MaxAttributes.Attribute[EffectLocation.Mana])
                {
                    player.Attributes.Attribute[EffectLocation.Mana] =
                        player.MaxAttributes.Attribute[EffectLocation.Mana];
                }
            }

            if (player.Attributes.Attribute[EffectLocation.Moves] <
                player.MaxAttributes.Attribute[EffectLocation.Moves])
            {
                player.Attributes.Attribute[EffectLocation.Moves] += Formulas.GainAmount(moves, player);
                if (player.Attributes.Attribute[EffectLocation.Moves] >
                    player.MaxAttributes.Attribute[EffectLocation.Moves])
                {
                    player.Attributes.Attribute[EffectLocation.Moves] =
                        player.MaxAttributes.Attribute[EffectLocation.Moves];
                }
            }

            Handler.Client.UpdateHP(player);
            Handler.Client.UpdateMana(player);
            Handler.Client.UpdateMoves(player);
            Handler.Client.UpdateScore(player);
        }
    }
}