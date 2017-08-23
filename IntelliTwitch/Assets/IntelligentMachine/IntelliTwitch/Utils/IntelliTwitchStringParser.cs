using System.Linq;

namespace IntelligentMachine.Twitch.IRC
{
    public static class IntelliTwitchStringParser
    {
        /// <summary>
        /// Returns the display-name from Twitch.
        /// Defaults to retrieving from USERNAME!USERNAME@tmi.twitch.tv, if a display-name
        /// is not set. Returns string.Empty if nothing is found.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static string GetDisplayName(string[] userInfo)
        {
            for (var i = 0; i < userInfo.Length; i++)
            {
                if (userInfo[i].Contains("display-name"))
                {
                    var userSplit = userInfo[i].Split('=').Last();

                    //Display name can be blank, so if it hasn't been set we can grab it from another part of the message
                    if (!string.IsNullOrEmpty(userSplit))
                        return userSplit;

                    for (int x = 0; x < userInfo.Length; x++)
                    {
			            if (userInfo[x].Contains("tmi.twitch.tv"))
                        {
                            return userInfo[x].Split('!').First();
                        } 
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Returns the color of the username the user has set. Defaults to black if none.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static string GetUserNameColor(string[] userInfo)
        {
            string cache = string.Empty;

            for (var i = 0; i < userInfo.Length; i++)
            {
                if (userInfo[i].Contains("color"))
                    cache = userInfo[i].Split('=').Last();
            }

            return string.IsNullOrEmpty(cache) ? "#000000" : cache;
        }

        /// <summary>
        /// Returns true if the user is a subscriber
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool GetSubscriberStatus(string[] userInfo)
        {
            for (var i = 0; i < userInfo.Length; i++)
            {
                if (userInfo[i].Contains("subscriber"))
                    return userInfo[i].Split('=').Last().Contains("1");
            }

            return false;
        }

        /// <summary>
        /// Returns true if the user is a Twitch Turbo member
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool GetTurboStatus(string[] userInfo)
        {
            for (var i = 0; i < userInfo.Length; i++)
            {
                if (userInfo[i].Contains("turbo"))
                    return userInfo[i].Split('=').Last().Contains("1");
            }

            return false;
        }

        /// <summary>
        /// Returns true if the user is a moderator of the channel
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static bool GetChannelModStatus(string[] userInfo)
        {
            for (var i = 0; i < userInfo.Length; i++)
            {
                //Give mod status to the broadcaster as well
                if (userInfo[i].Contains("@badges"))
                {
                    if (userInfo[i].Split('=').Last().Contains("broadcaster"))
                        return true;
                }
                if (userInfo[i].Contains("mod"))
                    return userInfo[i].Split('=').Last().Contains("1");
            }

            return false;
        }

        /// <summary>
        /// Returns the user-type from Twitch
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static string GetUserType(string[] userInfo)
        {
            for (var i = 0; i < userInfo.Length; i++)
            {
                if (userInfo[i].Contains("user-type"))
                    return userInfo[i].Split('=').Last();
            }

            return string.Empty;
        }

        /// <summary>
        /// Will build a fancy string for nicer looking debug code.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string BuildDebugMessage(string sender, string msg)
        {
            return string.Format("[IntelliTwitch][{0}] {1}", sender, msg);
        }
    }
}