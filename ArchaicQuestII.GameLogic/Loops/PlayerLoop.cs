using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using System.Linq;

namespace ArchaicQuestII.GameLogic.Loops
{
    public class PlayerLoop : ILoop
    {
        public int TickDelay => 120000;
        public bool ConfigureAwait => false;
        private List<Player> _players = new List<Player>();

        public void PreTick()
        {
            _players = Services.Instance.Cache.GetPlayerCache().Values.ToList();
        }

        public void Tick()
        {
            foreach (var player in _players)
            {
                //  IdleCheck(player);

                var hP = (DiceBag.Roll(1, 2, 5));
                var mana = (DiceBag.Roll(1, 2, 5));
                var moves = (DiceBag.Roll(1, 2, 5));

                // if player has fast healing add the bonus here
                var hasFastHealing = player.Skills.FirstOrDefault(
                    x => x.Name == SkillName.FastHealing && player.Level >= x.Level
                );

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

                if (
                    player.Attributes.Attribute[EffectLocation.Hitpoints]
                    < player.MaxAttributes.Attribute[EffectLocation.Hitpoints]
                )
                {
                    if (hasFastHealing != null)
                    {
                        if (player.RollSkill(SkillName.FastHealing, false))
                        {
                            hP *= 2;
                        }
                        else
                        {
                            player.FailedSkill(SkillName.FastHealing, false);
                        }
                    }

                    player.Attributes.Attribute[EffectLocation.Hitpoints] += GainAmount(hP, player);
                    if (
                        player.Attributes.Attribute[EffectLocation.Hitpoints]
                        > player.MaxAttributes.Attribute[EffectLocation.Hitpoints]
                    )
                    {
                        player.Attributes.Attribute[EffectLocation.Hitpoints] = player
                            .MaxAttributes
                            .Attribute[EffectLocation.Hitpoints];
                    }
                }

                if (
                    player.Attributes.Attribute[EffectLocation.Mana]
                    < player.MaxAttributes.Attribute[EffectLocation.Mana]
                )
                {
                    player.Attributes.Attribute[EffectLocation.Mana] += GainAmount(mana, player);

                    if (
                        player.Attributes.Attribute[EffectLocation.Mana]
                        > player.MaxAttributes.Attribute[EffectLocation.Mana]
                    )
                    {
                        player.Attributes.Attribute[EffectLocation.Mana] = player
                            .MaxAttributes
                            .Attribute[EffectLocation.Mana];
                    }
                }

                if (
                    player.Attributes.Attribute[EffectLocation.Moves]
                    < player.MaxAttributes.Attribute[EffectLocation.Moves]
                )
                {
                    player.Attributes.Attribute[EffectLocation.Moves] += GainAmount(moves, player);
                    if (
                        player.Attributes.Attribute[EffectLocation.Moves]
                        > player.MaxAttributes.Attribute[EffectLocation.Moves]
                    )
                    {
                        player.Attributes.Attribute[EffectLocation.Moves] = player
                            .MaxAttributes
                            .Attribute[EffectLocation.Moves];
                    }
                }

                player.UpdateClientUI();
            }
        }

        public void PostTick()
        {
            _players.Clear();
        }

        private int GainAmount(int value, Player player)
        {
            return player.Status switch
            {
                CharacterStatus.Status.Sleeping => value *= 3,
                CharacterStatus.Status.Resting => value *= 2,
                _ => value
            };
        }
    }
}
