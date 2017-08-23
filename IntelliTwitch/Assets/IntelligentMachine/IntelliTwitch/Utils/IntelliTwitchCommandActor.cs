namespace IntelligentMachine.Twitch.IRC
{
    public struct IntelliTwitchCommandActor
    {
        private IntelliTwitchUser user;
        
        public string UserName
        {
            get
            {
                return user.UserName;
            }
        }

        public string Channel { get; private set; }

        public IntelliTwitchCommandActor(IntelliTwitchUser sender, string channelActedOn)
        {
            user = sender;
            Channel = channelActedOn;
        }
    }
}
