namespace IntelligentMachine.Twitch.IRC
{
    public interface IIntelliTwitchCommand
    {
        string command { get; }
        bool modRequired { get; }
        IIntelliTwitchCommandAction BuildCommand(IntelliTwitchCommandActor actor);
    }
}
