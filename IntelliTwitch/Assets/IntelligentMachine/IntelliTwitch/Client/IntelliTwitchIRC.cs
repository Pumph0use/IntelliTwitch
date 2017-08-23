using System.Threading;
using DisruptorUnity3d;
using System.IO;
using System.Net.Sockets;
using System;
using System.Diagnostics;

namespace IntelligentMachine.Twitch.IRC
{
    public class IntelliTwitchIRC
    {
        #region VARIABLES

        private IntelliTwitchBotInfo botInfo;
        private IntelliTwitchInterpreter interpreter;

        private Thread inputThread;
        private Thread outputThread;

        private RingBuffer<string> outgoingBuffer; //Hold all of our raw data from chat, another class will handle converting into something game friendly - IntelliTwitchInterpreter
        private RingBuffer<string> commandBuffer; //holds all commands that need to be sent TO twitch via IRC.
        private RingBuffer<IIntelliTwitchCommandAction> incomingBuffer; //Recieves the processed information from the interpreter.

#if UNITY_EDITOR
        private RingBuffer<string> debugBuffer;
#endif

        private StreamReader inputReader;
        private StreamWriter outputWriter;

        private string chatCache;

        private TcpClient connection;

        private bool stopThreads;

        #endregion

        #region CONSTRUCTORS

        public IntelliTwitchIRC(IntelliTwitchBotInfo info, string databaseName)
        {
            botInfo = info;
            interpreter = new IntelliTwitchInterpreter(databaseName, this);
            Init();
        }

        #endregion

        #region INITIALIZATION

        private void Init()
        {
            stopThreads = false;

            outgoingBuffer = new RingBuffer<string>(botInfo.bufferSize);
            commandBuffer = new RingBuffer<string>(botInfo.bufferSize);
            incomingBuffer = new RingBuffer<IIntelliTwitchCommandAction>(botInfo.bufferSize);

#if UNITY_EDITOR
            debugBuffer = new RingBuffer<string>(botInfo.bufferSize);
#endif

#if UNITY_EDITOR
            SendDebugMessage("Client Initialized!");

#endif
            connection = new TcpClient(GlobalSettings.host, GlobalSettings.port);
            if (!connection.Connected)
            {
                throw new NotImplementedException();
            }

#if UNITY_EDITOR
            SendDebugMessage("Connection Established!");

#endif
            var networkStream = connection.GetStream();
            inputReader = new StreamReader(networkStream);
            outputWriter = new StreamWriter(networkStream);

            outputWriter.WriteLine("PASS " + GlobalSettings.OAUTH);
            outputWriter.WriteLine("NICK " + botInfo.botUserName.ToLower());
            outputWriter.Flush();

            inputThread = new Thread(() => ProcessInput(inputReader, networkStream));
            inputThread.Start();

            outputThread = new Thread(() => ProcessOutput(outputWriter));
            outputThread.Start();

        }
        
        #endregion

        #region THREADING

        //Runs on the Input Thread
        private void ProcessInput(TextReader input, NetworkStream stream)
        {
            while (!stopThreads)
            {
                if (!stream.DataAvailable)
                    continue;

                chatCache = input.ReadLine();

#if UNITY_EDITOR
                //UNCOMMENT FOR RAW FEED
                //SendDebugMessage(chatCache);

#endif
                if (chatCache.StartsWith("PING "))
                {
#if UNITY_EDITOR
                    SendDebugMessage(chatCache.Replace("PING", "PONG"));
#endif
                    SendCommand(chatCache.Replace("PING", "PONG"));
                }

                //After server sends 001 command, we can join a channel
                else if (chatCache.Split(' ')[1] == "001")
                {

#if UNITY_EDITOR
                    if (botInfo.channelsToJoin.Count == 0 || botInfo.channelsToJoin == null)
                        SendDebugMessage("Connected with no channels to join! Add channels to the list and restart to connect.");
#endif

                    //send request for permissions before joining the channel!!!  TAGS, MEMBERSHIP, COMMANDS
                    SendCommand("CAP REQ :twitch.tv/membership twitch.tv/tags twitch.tv/commands");
                    for (var i = 0; i < botInfo.channelsToJoin.Count; i++)
                    {
                        SendCommand("JOIN #" + botInfo.channelsToJoin[i]);
                        SendMessage(botInfo.channelsToJoin[i], "Hello World!! How are you today?");
#if UNITY_EDITOR
                        SendDebugMessage("Joining Channel #" + botInfo.channelsToJoin[i]);
#endif
                    }
                }
                else
                    SendToInterpreter(chatCache);

            }
        }

        //Runs on the Output Thread
        private void ProcessOutput(TextWriter output)
        {
            while (!stopThreads)
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                while (!stopThreads)
                {

                    //TODO: Make bot check if its modded, and adjust the limits accordingly.
                    if (stopWatch.ElapsedMilliseconds > 1750)
                    {
                        string cmdCache;
                        if (commandBuffer.TryDequeue(out cmdCache))
                        {
                            output.WriteLine(cmdCache);
#if UNITY_EDITOR
                            SendDebugMessage("CMD SENT: " + cmdCache);
#endif
                            output.Flush();
                            stopWatch.Reset();
                            stopWatch.Start();
                        }
                    }

                    string outCache;
                    if (outgoingBuffer.TryDequeue(out outCache))
                    {
                        interpreter.Interpret(outCache);
                    }
                }
            }
        }

       

        public void Stop()
        {
            stopThreads = true;
            inputThread.Abort();
            outputThread.Abort();
        }

        #endregion

        #region Public API

        /// <summary>
        /// Sends provided message to the designated channel.
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="msg"></param>
        public void SendMessage(string channelName, string msg)
        {
            commandBuffer.Enqueue("PRIVMSG #" + channelName + " :" + msg);
        }

        /// <summary>
        /// Sends the provided message as a whisper to the user on a provided channel.
        /// NOTE: ONLY WORKS IF USER IS FOLLOWING THE BOT ACCOUNT.
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="user"></param>
        /// <param name="msg"></param>
        //TODO: Determine how to remove the requirement of following the bot account to recieve whispers.
        public void SendWhisper(string channelName, string user, string msg)
        {
            //Must be sent through a PRIVMSG
            SendMessage(channelName, "/w " + user + " " + msg);
        }

        /// <summary>
        /// Adds the designated command action to the queue to be processed.
        /// </summary>
        /// <param name="cmd"></param>
        public void QueueProcessedAction(IIntelliTwitchCommandAction cmd)
        {
            incomingBuffer.Enqueue(cmd);
        }

        /// <summary>
        /// Returns the next available command, returns null if not available.
        /// </summary>
        /// <returns></returns>
        public IIntelliTwitchCommandAction RetrieveNextCommandAction()
        {
            IIntelliTwitchCommandAction temp;
            if (incomingBuffer.TryDequeue(out temp))
                return temp;

            return null;
        }

        #endregion

        #region Private API

        private void SendCommand(string cmd)
        {
            commandBuffer.Enqueue(cmd);
        }

        private void SendToInterpreter(string msg)
        {
            outgoingBuffer.Enqueue(msg);
        }

        private IIntelliTwitchCommandAction DequeueProcessedBuffer()
        {
            IIntelliTwitchCommandAction temp;
            if (incomingBuffer.TryDequeue(out temp))
                return temp;

            return null;
        }

        #endregion

#if UNITY_EDITOR
        #region DEBUG LOGIC

        /*
         * This region is entirely for testing purposes, so I have it flagged to not compile unless it is in the unity editor.
         * Remove all of the #if UNITY_EDITOR sections if you want it in a build.
         */

        /// <summary>
        /// Returns the next debug message in the Debug queue. Returns null if unable to dequeue currently, or nothing in queue
        /// </summary>
        /// <returns></returns>
        public string DequeueDebug()
        {
            string cache;
            if (debugBuffer.TryDequeue(out cache))
                return cache;

            return null;
        }

        private void SendDebugMessage(string msg)
        {
            debugBuffer.Enqueue(IntelliTwitchStringParser.BuildDebugMessage("IRCClient", msg));
        }

        #endregion
#endif
    }
}
