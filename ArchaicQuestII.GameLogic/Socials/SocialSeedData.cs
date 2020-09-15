using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character.Emote;

namespace ArchaicQuestII.GameLogic.Socials
{
    public class SocialSeedData
    {
        public Dictionary<string, Emote> SeedData()
        {
            var seed = new Dictionary<string, Emote>
            {
                {"accuse", new Emote()
                {
                    CanTarget = true,
                    ToRoom = "# looks accusingly at @.",
                    ToSender = "You look accusingly at @.",
                    ToSenderAtTarget = "You look accusingly at @.",
                    ToTarget = "# looks accusingly at you.",
                    ToRoomTarget = "# looks accusingly at @."
                }},
                {"ack", new Emote()
                {
                    CanTarget = true,
                    ToRoom = "# ACKS at % big mistake.",
                    ToSender = "You gasp and say 'ACK!' at your mistake.",
                    ToSenderAtTarget = "You gasp and say 'ACK!' to @.",
                    ToTarget = "# gasps says 'ACK!' to you.",
                    ToRoomTarget = "# gasps and says 'ACK!' to @."
                }},
                {"adore", new Emote()
                {
                    CanTarget = true,
                    ToRoom = "# is looking for someone to adore!",
                    ToSender = "You are looking for someone to adore!",
                    ToSenderAtTarget = "You look at @ with adoring eyes.",
                    ToTarget = "# looks at you with adoring eyes.",
                    ToRoomTarget = "# looks at @ with adoring eyes."
                }},
                {"agree", new Emote()
                {
                    ToRoom = "# seems to agree.",
                    ToSender = "You seem to be in an agreeable mood.",
                    ToSenderAtTarget = "You agree with @.",
                    ToTarget = "# agrees with you.",
                    ToRoomTarget = "# agrees with @."
                }},
                {"amused", new Emote()
                {
                    ToRoom = "# gives an amused look.",
                    ToSender = "You seem amused about something.",
                    ToSenderAtTarget = "You give @ an amused grin.",
                    ToTarget = "# gives you an amused grin.",
                    ToRoomTarget = "# gives an amused grin at @."
                }},
                {"arf", new Emote()
                {
                    ToRoom = "# exclaims, \"Arf! Arf! Arf!\"",
                    ToSender = "You exclaim, \"Arf! Arf! Arf!\"",
                    ToSenderAtTarget = "You look at @ and exclaim, \"Arf! Arf! Arf!\"",
                    ToTarget = "# looks at you and exclaims, \"Arf! Arf! Arf!\"",
                    ToRoomTarget = "# at @ and exclaims, \"Arf! Arf! Arf!\""
                }},
                {"baffle", new Emote()
                {
                    ToRoom = "# scrunches up % face because %% is totally baffled!",
                    ToSender = "You scrunch up your face because you are totally baffled!",
                    ToSenderAtTarget = "You scrunch up your face at @ because % behavior totally baffles you!",
                    ToTarget = "# scrunches up % face at you because you totally baffle %!",
                    ToRoomTarget = "# scrunches up #% face at @ because @%% totally baffle #%!"
                }},
                {"bashful", new Emote()
                {
                    ToRoom = "# begins looking quite bashful.",
                    ToSender = "For some reason, you start feeling very bashful.",
                    ToSenderAtTarget = "You look up at @, look at the ground and bashfully trace figure eights with your foot.",
                    ToTarget = "# looks up at you, then bashfully traces figure eights with her foot.",
                    ToRoomTarget = "# looks up at #, then bashfully traces figure eights with her foot."
                }},
                {"blank", new Emote()
                {
                    ToRoom = "# gets a blank look on #% face.",
                    ToSender = "You get a blank look on your face.",
                    ToSenderAtTarget = "You look at @ but draw a complete blank.",
                    ToTarget = "# looks at you and draws a total blank.",
                    ToRoomTarget = "# looks at @ and draws a total blank."
                }},
                {"clap", new Emote()
                {
                    ToRoom = "# claps.",
                    ToSender = "You clap.",
                    ToSenderAtTarget = "You clap for @.",
                    ToTarget = "# claps for you.",
                    ToRoomTarget = "# claps for @."

                }},
                    {"smile", new Emote()
                    {

                        ToRoom = "# smiles.",
                        ToSender = "You smile.",
                        ToSenderAtTarget = "You smile at @.",
                        ToTarget = "# smiles at you.",
                        ToRoomTarget = "# smiles at @."

                    }
                }
            };

            return seed;
        }
    }
}
