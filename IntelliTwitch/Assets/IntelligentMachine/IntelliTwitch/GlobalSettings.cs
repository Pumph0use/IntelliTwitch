using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace IntelligentMachine.Twitch.IRC
{
    public static class GlobalSettings
    {

        public static string OAUTH = "YOUR OAUTH CODE HERE";  //https://twitchapps.com/tmi/  - Go here while logged into the twitch account you want an oAuth key for.
        public static string host = "irc.twitch.tv";
        public static int port = 6667;  //443 if using SSL

    } 
}
