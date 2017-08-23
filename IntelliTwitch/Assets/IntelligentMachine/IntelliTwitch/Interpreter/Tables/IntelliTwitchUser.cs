using SQLite4Unity3d;

public class IntelliTwitchUser
{
    /*
     * @badges=; //broadcaster, etc.
     * color=#000000;
     * display-name=DISPLAYNAME;  CAN BE BLANK SOMETIMES
     * emotes=;
     * id=2c0a1be0-d2e5-456b-aebd-45611488c31c;
     * mod=0;
     * room-id=165532076;
     * subscriber=0;
     * tmi-sent-ts=1500448009323;
     * turbo=0;
     * user-id=69525992;
     * user-type= 
     * :USERNAME!USERNAME@USERNAME.tmi.twitch.tv PRIVMSG #CHANNEL : MESSAGE
     * 
     * see 
     */

    [PrimaryKey]
    public int ID { get; set; } //Twitch user ID
    public string UserName { get; set; } //from display-name, if empty, from the @ symbol.
    public string UserNameColor { get; set; } //from color
    public bool IsSubscriber { get; set; }
    public bool IsTurbo { get; set; }
    public bool IsChannelMod { get; set; }
    public int UserTypeID { get; set; }
    public int joinDate { get; set; }
}
