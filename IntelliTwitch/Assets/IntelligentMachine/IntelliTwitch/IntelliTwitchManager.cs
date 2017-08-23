using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IntelligentMachine.Twitch.IRC
{
    public class IntelliTwitchManager : ManagerBase<IntelliTwitchManager>
    {

        public IntelliTwitchBotInfo botInfo;
        public string databaseName;
        public IntelliTwitchIRC chatClient;

#if UNITY_EDITOR
        public bool logDebugToConsole; 
#endif

        #region Start/Awake

        public void Start()
        {
            chatClient = new IntelliTwitchIRC(botInfo, databaseName);
        }

        #endregion

        #region Update

        public void Update()
        {
#if UNITY_EDITOR
            if (logDebugToConsole)
            {
                var tempDebug = chatClient.DequeueDebug();
                if (!string.IsNullOrEmpty(tempDebug))
                    Debug.Log(tempDebug); 
            }
#endif
        }

        #endregion

        #region Public API

        /// <summary>
        /// Sends a message to the specified channel.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        public void SendChatMessage(string channel, string msg)
        {
            chatClient.SendMessage(channel, msg);
        }

        //TODO: Find a way to whisper the user if they are not following
        /// <summary>
        /// Sends a whisper to specified user. NOTE: CURRENTLY ONLY WORKS IF THEY ARE FOLLOWING THE BOT ACCOUNT
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="user"></param>
        /// <param name="msg"></param>
        public void SendChatWhisper(string channel, string user, string msg)
        {
            SendChatMessage(channel, "/w " + user + " " + msg);
        }

        #endregion

        #region Editor Methods
        
        /// <summary>
        /// Creates AND Erases the current database.
        /// Use for first time creation of the database.
        /// </summary>
        [ContextMenu("Build New Database")]
        public void BuildDatabase()
        {
            Debug.Log(IntelliTwitchStringParser.BuildDebugMessage("Manager", "Creating new database..."));
            var dataServ = new IntelliTwitchDataService("HouseOfDungeons.db");
            dataServ.CreateDB();
        }

        #endregion

        #region Program Stop

        private void OnDisable()
        {
            chatClient.Stop();
        }

        private void OnDestroy()
        {
            chatClient.Stop();
        }

        #endregion

    }
}

