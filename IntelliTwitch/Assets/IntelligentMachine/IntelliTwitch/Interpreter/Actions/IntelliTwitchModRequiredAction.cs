namespace IntelligentMachine.Twitch.IRC
{
    public class IntelliTwitchModRequiredAction : IntelliTwitchCommandAction
    {
        public IntelliTwitchModRequiredAction(IntelliTwitchCommandActor user) : base(user)
        {

        }

        public override void RunCommand()
        {
            //TODO: return the command attempted to be used.
            IntelliTwitchManager.Instance.SendChatWhisper(sender.Channel, sender.UserName, "You require Moderator status to use that command.");
        }
    }
}
