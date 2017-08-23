using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IntelligentMachine.Twitch.IRC
{
    public class FunCommandAction : IntelliTwitchCommandAction
    {
        public FunCommandAction(IntelliTwitchCommandActor user) : base(user)
        {
        }

        public override void RunCommand()
        {
            for(var i = 10; i > 0; i--)
                IntelliTwitchManager.Instance.SendChatMessage(sender.Channel, "You will explode in T-Minus: " + i);

            IntelliTwitchManager.Instance.SendChatMessage(sender.Channel, "You got deaded " + sender.UserName);

        }
    }
}
