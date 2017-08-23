using System;
using UnityEngine;

namespace IntelligentMachine.Twitch.IRC
{
    public class DebugTestCommandAction : IntelliTwitchCommandAction
    {
        public DebugTestCommandAction(IntelliTwitchCommandActor user) : base(user)
        {

        }

        public override void RunCommand()
        {
            IntelliTwitchManager.Instance.SendChatMessage(sender.Channel, "You have entered the Debug Test command!");
            IntelliTwitchManager.Instance.SendChatMessage(sender.Channel, sender.UserName + " You're Winner!");
        }
    }
}
