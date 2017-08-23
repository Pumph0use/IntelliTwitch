using System;

namespace IntelligentMachine.Twitch.IRC
{
    public class ReflectionTestCommand : IIntelliTwitchCommand
    {
        public string command
        {
            get
            {
                return "!reflect";
            }
        }

        public bool modRequired
        {
            get
            {
                return true;
            }
        }

        public IIntelliTwitchCommandAction BuildCommand(IntelliTwitchCommandActor actor)
        {
            return new ReflectionTestCommandAction(actor);
        }
    }
}
