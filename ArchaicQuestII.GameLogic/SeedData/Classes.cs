using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.SeedData
{
    public static class Classes
    {
        public static void SetGenericTitle(Player player)
        {
            var prevtitle = string.Empty;
            var title = string.Empty;

            if (player.Gender == "Male")
            {
                title = GitTitleMale(player.ClassName.ToString(), player.Level);
                prevtitle = GitTitleMale(player.ClassName.ToString(), player.Level - 1);
            }
            else
            {
                title = GetTitleFemale(player.ClassName.ToString(), player.Level);
                prevtitle = GetTitleFemale(player.ClassName.ToString(), player.Level - 1);
            }

            if (!string.IsNullOrEmpty(player.Title) && player.Title != prevtitle)
            {
                // custom title set so don't set a default one
                return;
            }

            player.Title = title;
        }

        private static string GetTitleFemale(string className, int level)
        {
            switch (className)
            {
                case "Fighter":
                    switch (level)
                    {
                        case 1:
                        case 2:
                        case 3:
                            return "the Swordpupil";
                        case 4:
                        case 5:
                        case 6:
                            return "the Recruit";
                        case 7:
                        case 8:
                        case 9:
                            return "the Sentress";
                        case 10:
                        case 11:
                        case 12:
                            return "the Fighter";
                        case 13:
                        case 14:
                        case 15:
                            return "the Soldier";
                        case 16:
                        case 17:
                        case 18:
                            return "the Warrior";
                        case 19:
                        case 20:
                        case 21:
                            return "the Veteran";
                        case 22:
                        case 23:
                        case 24:
                            return "the Swordswoman";
                        case 25:
                        case 26:
                        case 27:
                            return "the Fenceress";
                        case 28:
                        case 29:
                        case 30:
                            return "the Combatess";
                        case 31:
                        case 32:
                        case 33:
                            return "the Heroine";
                        case 34:
                        case 35:
                        case 36:
                            return "the Myrmidon";
                        case 37:
                        case 38:
                        case 39:
                            return "the Swashbuckleress";
                        case 40:
                        case 41:
                        case 42:
                            return "the Mercenaress";
                        case 43:
                        case 44:
                        case 45:
                            return "the Swordmistress";
                        case 46:
                        case 47:
                        case 48:
                            return "the Lieutenant";
                        case 49:
                        case 50:
                        case 51:
                            return "the Lady Champion";

                        default:
                            return "the Warrior";
                    }
                case "Mage":
                    switch (level)
                    {
                        case 1:
                        case 2:
                        case 3:
                            return "the Apprentice of Magic";
                        case 4:
                        case 5:
                        case 6:
                            return "the Spell Student";
                        case 7:
                        case 8:
                        case 9:
                            return "the Scholar of Magic";
                        case 10:
                        case 11:
                        case 12:
                            return "the Delveress in Spells";
                        case 13:
                        case 14:
                        case 15:
                            return "the Medium of Magic";
                        case 16:
                        case 17:
                        case 18:
                            return "the Scribess of Magic";
                        case 19:
                        case 20:
                        case 21:
                            return "the Seeress";
                        case 22:
                        case 23:
                        case 24:
                            return "the Sage";
                        case 25:
                        case 26:
                        case 27:
                            return "the Illusionist";
                        case 28:
                        case 29:
                        case 30:
                            return "the Abjuress";
                        case 31:
                        case 32:
                        case 33:
                            return "the Invoker";
                        case 34:
                        case 35:
                        case 36:
                            return "the Enchantress";
                        case 37:
                        case 38:
                        case 39:
                            return "the Conjuress";
                        case 40:
                        case 41:
                        case 42:
                            return "the Witch";
                        case 43:
                        case 44:
                        case 45:
                            return "the War Witch";
                        case 46:
                        case 47:
                        case 48:
                            return "the Sorceress";
                        case 49:
                        case 50:
                        case 51:
                            return "Archwitch";

                        default:
                            return "the Wizard";
                    }
                case "Rogue":
                    switch (level)
                    {
                        case 1:
                        case 2:
                        case 3:
                            return "the Pilferess";
                        case 4:
                        case 5:
                        case 6:
                            return "the Footpad";
                        case 7:
                        case 8:
                        case 9:
                            return "the Filcheress";
                        case 10:
                        case 11:
                        case 12:
                            return "the Pick-Pocket";
                        case 13:
                        case 14:
                        case 15:
                            return "the Sneak";
                        case 16:
                        case 17:
                        case 18:
                            return "the Pincheress";
                        case 19:
                        case 20:
                        case 21:
                            return "the Cut-Purse";
                        case 22:
                        case 23:
                        case 24:
                            return "the Snatcheress";
                        case 25:
                        case 26:
                        case 27:
                            return "the Sharpress";
                        case 28:
                        case 29:
                        case 30:
                            return "the Rogue";
                        case 31:
                        case 32:
                        case 33:
                            return "the Robber";
                        case 34:
                        case 35:
                        case 36:
                            return "the Magswoman";
                        case 37:
                        case 38:
                        case 39:
                            return "the Highwaywoman";
                        case 40:
                        case 41:
                        case 42:
                            return "the Thief";
                        case 43:
                        case 44:
                        case 45:
                            return "the quick-blade";
                        case 46:
                        case 47:
                        case 48:
                            return "the Murderess";
                        case 49:
                        case 50:
                        case 51:
                            return "the Cut-Throat";

                        default:
                            return "the Assasin";
                    }
                case "Cleric":
                    switch (level)
                    {
                        case 1:
                        case 2:
                        case 3:
                            return "the Believer";
                        case 4:
                        case 5:
                        case 6:
                            return "the Acolyte";
                        case 7:
                        case 8:
                        case 9:
                            return "the Novice";
                        case 10:
                        case 11:
                        case 12:
                            return "the Missionary";
                        case 13:
                        case 14:
                        case 15:
                            return "the Adept";
                        case 16:
                        case 17:
                        case 18:
                            return "the Deaconess";
                        case 19:
                        case 20:
                        case 21:
                            return "the Vicaress";
                        case 22:
                        case 23:
                        case 24:
                            return "the Priestess";
                        case 25:
                        case 26:
                        case 27:
                            return "the Lady Minister";
                        case 28:
                        case 29:
                        case 30:
                            return "the Levitess";
                        case 31:
                        case 32:
                        case 33:
                            return "the Curess";
                        case 34:
                        case 35:
                        case 36:
                            return "the Chaplain";
                        case 37:
                        case 38:
                        case 39:
                            return "the Bishop";
                        case 40:
                        case 41:
                        case 42:
                            return "the Holy";
                        case 43:
                        case 44:
                        case 45:
                            return "the Nunne";
                        case 46:
                        case 47:
                        case 48:
                            return "the Arch Lady of the Church";
                        case 49:
                        case 50:
                        case 51:
                            return "the Matriarch";

                        default:
                            return "the Assasin";
                    }
            }

            return string.Empty;
        }

        private static string GitTitleMale(string className, int level)
        {
            switch (className)
            {
                case "Fighter":
                    switch (level)
                    {
                        case 1:
                        case 2:
                        case 3:
                            return "the Swordpupil";
                        case 4:
                        case 5:
                        case 6:
                            return "the Recruit";
                        case 7:
                        case 8:
                        case 9:
                            return "the Sentry";
                        case 10:
                        case 11:
                        case 12:
                            return "the Fighter";
                        case 13:
                        case 14:
                        case 15:
                            return "the Soldier";
                        case 16:
                        case 17:
                        case 18:
                            return "the Warrior";
                        case 19:
                        case 20:
                        case 21:
                            return "the Veteran";
                        case 22:
                        case 23:
                        case 24:
                            return "the Swordmaster";
                        case 25:
                        case 26:
                        case 27:
                            return "the Fencer";
                        case 28:
                        case 29:
                        case 30:
                            return "the Combatant";
                        case 31:
                        case 32:
                        case 33:
                            return "the Hero";
                        case 34:
                        case 35:
                        case 36:
                            return "the Myrmidon";
                        case 37:
                        case 38:
                        case 39:
                            return "the Swashbuckler";
                        case 40:
                        case 41:
                        case 42:
                            return "the Mercenary";
                        case 43:
                        case 44:
                        case 45:
                            return "the Swordmaster";
                        case 46:
                        case 47:
                        case 48:
                            return "the Lieutenant";
                        case 49:
                        case 50:
                        case 51:
                            return "the Champion";

                        default:
                            return "the Warrior";
                    }
                case "Mage":
                    switch (level)
                    {
                        case 1:
                        case 2:
                        case 3:
                            return "the Apprentice of Magic";
                        case 4:
                        case 5:
                        case 6:
                            return "the Spell Student";
                        case 7:
                        case 8:
                        case 9:
                            return "the Scholar of Magic";
                        case 10:
                        case 11:
                        case 12:
                            return "the Delver in Spells";
                        case 13:
                        case 14:
                        case 15:
                            return "the Medium of Magic";
                        case 16:
                        case 17:
                        case 18:
                            return "the Scribess of Magic";
                        case 19:
                        case 20:
                        case 21:
                            return "the Seer";
                        case 22:
                        case 23:
                        case 24:
                            return "the Sage";
                        case 25:
                        case 26:
                        case 27:
                            return "the Illusionist";
                        case 28:
                        case 29:
                        case 30:
                            return "the Abjurer";
                        case 31:
                        case 32:
                        case 33:
                            return "the Invoker";
                        case 34:
                        case 35:
                        case 36:
                            return "the Enchanter";
                        case 37:
                        case 38:
                        case 39:
                            return "the Conjurer";
                        case 40:
                        case 41:
                        case 42:
                            return "the Magician";
                        case 43:
                        case 44:
                        case 45:
                            return "the Magus";
                        case 46:
                        case 47:
                        case 48:
                            return "the Wizard";
                        case 49:
                        case 50:
                        case 51:
                            return "Archmage";

                        default:
                            return "the Wizard";
                    }
                case "Rogue":
                    switch (level)
                    {
                        case 1:
                        case 2:
                        case 3:
                            return "the Pilferer";
                        case 4:
                        case 5:
                        case 6:
                            return "the Footpad";
                        case 7:
                        case 8:
                        case 9:
                            return "the Filcher";
                        case 10:
                        case 11:
                        case 12:
                            return "the Pick-Pocket";
                        case 13:
                        case 14:
                        case 15:
                            return "the Sneak";
                        case 16:
                        case 17:
                        case 18:
                            return "the Pincher";
                        case 19:
                        case 20:
                        case 21:
                            return "the Cut-Purse";
                        case 22:
                        case 23:
                        case 24:
                            return "the Snatcher";
                        case 25:
                        case 26:
                        case 27:
                            return "the Sharper";
                        case 28:
                        case 29:
                        case 30:
                            return "the Rogue";
                        case 31:
                        case 32:
                        case 33:
                            return "the Robber";
                        case 34:
                        case 35:
                        case 36:
                            return "the Magsman";
                        case 37:
                        case 38:
                        case 39:
                            return "the Highwayman";
                        case 40:
                        case 41:
                        case 42:
                            return "the Thief";
                        case 43:
                        case 44:
                        case 45:
                            return "the quick-blade";
                        case 46:
                        case 47:
                        case 48:
                            return "the killer";
                        case 49:
                        case 50:
                        case 51:
                            return "the Cut-Throat";

                        default:
                            return "the Assasin";
                    }
                case "Cleric":
                    switch (level)
                    {
                        case 1:
                        case 2:
                        case 3:
                            return "the Believer";
                        case 4:
                        case 5:
                        case 6:
                            return "the Acolyte";
                        case 7:
                        case 8:
                        case 9:
                            return "the Novice";
                        case 10:
                        case 11:
                        case 12:
                            return "the Missionary";
                        case 13:
                        case 14:
                        case 15:
                            return "the Adept";
                        case 16:
                        case 17:
                        case 18:
                            return "the Deacon";
                        case 19:
                        case 20:
                        case 21:
                            return "the Vicar";
                        case 22:
                        case 23:
                        case 24:
                            return "the Priest";
                        case 25:
                        case 26:
                        case 27:
                            return "the Minister";
                        case 28:
                        case 29:
                        case 30:
                            return "the Levite";
                        case 31:
                        case 32:
                        case 33:
                            return "the Curate";
                        case 34:
                        case 35:
                        case 36:
                            return "the Chaplain";
                        case 37:
                        case 38:
                        case 39:
                            return "the Healer";
                        case 40:
                        case 41:
                        case 42:
                            return "the Holy";
                        case 43:
                        case 44:
                        case 45:
                            return "the Bishop";
                        case 46:
                        case 47:
                        case 48:
                            return "the Arch Bishop";
                        case 49:
                        case 50:
                        case 51:
                            return "the Patriarch";

                        default:
                            return "the Assasin";
                    }
            }

            return string.Empty;
        }
    }
}
