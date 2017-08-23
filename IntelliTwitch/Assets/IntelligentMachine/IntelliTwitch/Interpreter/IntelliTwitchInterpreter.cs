using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace IntelligentMachine.Twitch.IRC
{
    public class IntelliTwitchInterpreter
    {
        private IntelliTwitchDataService dataService;
        private IntelliTwitchIRC client;

        private IIntelliTwitchCommand[] commandList;   //Created with reflection. Finds all commands created and in the project.

        #region Constructor

        public IntelliTwitchInterpreter(string serviceName, IntelliTwitchIRC chatClient)
        {
            dataService = new IntelliTwitchDataService(serviceName + ".db");
            client = chatClient;
            Init();
        }

        #endregion
        
        #region Initialization

        private void Init()
        {
            commandList = Utilities.BuildClassArrayFromInterface<IIntelliTwitchCommand>();
        }

        #endregion
        
        #region Public API

        public void Interpret(string msg)
        {
            if (msg.Contains("PRIVMSG"))
            {
                var splitCache = msg.Split(':'); //index 0 - should always be the user data. last index will be the PRIVMSG content;
                var content = splitCache[splitCache.Length - 1];

                //IE> !social, !create, !rank
                if (content.StartsWith("!")) //if it starts with a ! it is a command
                {
                    for (var i = 0; i < commandList.Length; i++)
                    {
                        if (content.Contains(commandList[i].command))
                        {
                            var user = GetUser(splitCache[0]);
                            if (CanExecuteCommand(user, commandList[i]))
                                client.QueueProcessedAction(commandList[i].BuildCommand(BuildCommandActor(user, GetChannel(splitCache))));
                            else
                                client.QueueProcessedAction(new IntelliTwitchModRequiredAction(BuildCommandActor(user, GetChannel(splitCache))));
                        }
                    }

                }

            }

        }

        #endregion

        #region Private Helper Methods

        private bool CanExecuteCommand(IntelliTwitchUser user, IIntelliTwitchCommand cmd)
        {
            if (cmd.modRequired)
                return user.IsChannelMod;
            else
                return true;
        }

        private IntelliTwitchCommandActor BuildCommandActor(IntelliTwitchUser user, string channel)
        {
            return new IntelliTwitchCommandActor(user, channel);
        }

        private string GetChannel(string[] info)
        {
            for (var i = 0; i < info.Length; i++)
            {
                if (info[i].Contains("PRIVMSG"))
                {
                    return info[i].Split('#').Last();
                }
            }

            return string.Empty;
        }

        private IntelliTwitchUser GetUser(string info)
        {
            var cache = info.Split(';');
            return dataService.GetUser(cache);
        }

        #endregion
    }
}
