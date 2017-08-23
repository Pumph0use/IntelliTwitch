using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IntelligentMachine.Twitch.IRC
{
    public class FunCommand : IIntelliTwitchCommand
    {
        public string command
        {
            get
            {
                return "!bomb";
            }
        }

        public bool modRequired { get { return false; } } 

        public IIntelliTwitchCommandAction BuildCommand(IntelliTwitchCommandActor actor)
        {
            return new FunCommandAction(actor);
        }
    }

}
