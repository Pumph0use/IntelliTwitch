namespace IntelligentMachine.Twitch.IRC
{
    public abstract class IntelliTwitchCommandAction : IIntelliTwitchCommandAction
    {
        protected IntelliTwitchCommandActor sender;

        public IntelliTwitchCommandAction(IntelliTwitchCommandActor user)
        {
            sender = user;
        }

        public abstract void RunCommand();
    }
}
