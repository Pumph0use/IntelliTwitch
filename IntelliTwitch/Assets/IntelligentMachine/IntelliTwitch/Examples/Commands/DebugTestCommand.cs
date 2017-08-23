using System;

namespace IntelligentMachine.Twitch.IRC
{
    public class DebugTestCommand : IIntelliTwitchCommand
    {
        public string command
        {
            get
            {
                return "!debug";
            }
        }

        public bool modRequired
        {
            get
            {
                return false;
            }
        }

        public IIntelliTwitchCommandAction BuildCommand(IntelliTwitchCommandActor actor)
        {
            return new DebugTestCommandAction(actor);
        }
    }
}
