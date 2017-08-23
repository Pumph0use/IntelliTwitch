using SQLite4Unity3d;

namespace IntelligentMachine.Twitch.IRC
{
    public class IntelliTwitchUserType
    {
        //The user’s type. Valid values: empty, mod, global_mod, admin, staff.
        //The broadcaster can have any of these.

        [PrimaryKey, AutoIncrement]
        public int UserTypeID { get; set; }
        public Enums.IntelliTwitchUserTypes UserType { get; set; }
        public string UserTypeName { get; set; }
} 
	}