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
                    CharNoTarget = "Accuse whom?",
                    RoomNoTarget = "#player# is in an accusing mood.",
                    TargetFound = "You look accusingly at #target#.",
                    ToTarget = "#player# looks accusingly at you.",
                    RoomTarget = "#player# looks accusingly at #target#.",
                    TargetSelf = "#player# seems to have a bad conscience.",
                    RoomSelf = "You accuse yourself."
                }},
                {"ack", new Emote()
                {
                    CharNoTarget = "You gasp and say 'ACK!' at your mistake.",
                    RoomNoTarget = "#player# ACKS at #pgender# big mistake.",
                    TargetFound = "You ACKS #target#.",
                    ToTarget = "#player# ACKS you.",
                    RoomTarget = "#player# ACKS #target#.",
                    TargetSelf = "#player# ACKS #pgender2#self. Must be a bad day.",
                    RoomSelf = "You ACKS yourself."
                }},
            //    {"adore", new Emote()
            //    {

            //        CharNoTarget = "#player# is looking for someone to adore!",
            //        ToSender = "You are looking for someone to adore!",
            //        ToSenderAtTarget = "You look at #target# with adoring eyes.",
            //        ToTarget = "#player# looks at you with adoring eyes.",
            //        ToRoomTarget = "#player# looks at #target# with adoring eyes."
            //    }},
            //    {"agree", new Emote()
            //    {
            //        CharNoTarget = "#player# seems to agree.",
            //        ToSender = "You seem to be in an agreeable mood.",
            //        ToSenderAtTarget = "You agree with #target#.",
            //        ToTarget = "#player# agrees with you.",
            //        ToRoomTarget = "#player# agrees with #target#."
            //    }},
            //    {"amused", new Emote()
            //    {
            //        CharNoTarget = "#player# gives an amused look.",
            //        ToSender = "You seem amused about something.",
            //        ToSenderAtTarget = "You give #target# an amused grin.",
            //        ToTarget = "#player# gives you an amused grin.",
            //        ToRoomTarget = "#player# gives an amused grin at #target#."
            //    }},
            //    {"arf", new Emote()
            //    {
            //        CharNoTarget = "#player# exclaims, \"Arf! Arf! Arf!\"",
            //        ToSender = "You exclaim, \"Arf! Arf! Arf!\"",
            //        ToSenderAtTarget = "You look at #target# and exclaim, \"Arf! Arf! Arf!\"",
            //        ToTarget = "#player# looks at you and exclaims, \"Arf! Arf! Arf!\"",
            //        ToRoomTarget = "#player# at #target# and exclaims, \"Arf! Arf! Arf!\""
            //    }},
            //    {"baffle", new Emote()
            //    {
            //        CharNoTarget = "#player# scrunches up % face because ? is totally baffled!",
            //        ToSender = "You scrunch up your face because you are totally baffled!",
            //        ToSenderAtTarget = "You scrunch up your face at #target# because ^ behavior totally baffles you!",
            //        ToTarget = "#player#player# scrunches up #pgender# face at you because you totally baffle #pgender2#!",
            //        ToRoomTarget = "#player# scrunches up % face at #target# because ^ totally baffle %!"
            //    }},
            //    {"bashful", new Emote()
            //    {
            //        CharNoTarget = "#player# begins looking quite bashful.",
            //        ToSender = "For some reason, you start feeling very bashful.",
            //        ToSenderAtTarget = "You look up at #target#, look at the ground and bashfully trace figure eights with your foot.",
            //        ToTarget = "#player# looks up at you, then bashfully traces figure eights with her foot.",
            //        ToRoomTarget = "#player# looks up at #, then bashfully traces figure eights with her foot."
            //    }},
            //    {"blank", new Emote()
            //    {
            //        CharNoTarget = "#player# gets a blank look on #% face.",
            //        ToSender = "You get a blank look on your face.",
            //        ToSenderAtTarget = "You look at #target# but draw a complete blank.",
            //        ToTarget = "#player# looks at you and draws a total blank.",
            //        ToRoomTarget = "#player# looks at #target# and draws a total blank."
            //    }},
            //    {"clap", new Emote()
            //    {
            //        CharNoTarget = "#player# claps.",
            //        ToSender = "You clap.",
            //        ToSenderAtTarget = "You clap for #target#.",
            //        ToTarget = "#player# claps for you.",
            //        ToRoomTarget = "#player# claps for #target#."

            //    }},
            //        {"smile", new Emote()
            //        {

            //            CharNoTarget = "#player# smiles.",
            //            ToSender = "You smile.",
            //            ToSenderAtTarget = "You smile at #target#.",
            //            ToTarget = "#player# smiles at you.",
            //            ToRoomTarget = "#player# smiles at #target#."

            //        }
            //    }
            };

            return seed;
        }
    }
}
