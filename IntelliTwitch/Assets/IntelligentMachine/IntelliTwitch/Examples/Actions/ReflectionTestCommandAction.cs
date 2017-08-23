namespace IntelligentMachine.Twitch.IRC
{
    public class ReflectionTestCommandAction : IntelliTwitchCommandAction
    {
        public ReflectionTestCommandAction(IntelliTwitchCommandActor user) : base(user)
        {
        }

        public override void RunCommand()
        {
            IntelliTwitchManager.Instance.SendChatMessage(sender.Channel, "Reflection test worked!");
        }
    }
}
