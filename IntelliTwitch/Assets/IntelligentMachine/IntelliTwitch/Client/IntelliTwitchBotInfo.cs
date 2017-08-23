using UnityEngine;
using System.Collections.Generic;

namespace IntelligentMachine.Twitch.IRC
{
    [CreateAssetMenu(fileName = "IntelliTwitchBotInfo", menuName = "IntelliTwitch/BotInfo")]
    public class IntelliTwitchBotInfo : ScriptableObject
    {
        public string botUserName;
        public List<string> channelsToJoin;
        public int bufferSize;

        public IntelliTwitchBotInfo(string userName, List<string> channels, int bufferAmt)
        {
            botUserName = userName;
            channelsToJoin = channels;
            bufferSize = bufferAmt;
        }
    } 
}