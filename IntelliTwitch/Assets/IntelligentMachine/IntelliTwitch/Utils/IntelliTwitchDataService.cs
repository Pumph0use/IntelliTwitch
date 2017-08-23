using SQLite4Unity3d;
using UnityEngine;
using System;
using System.Linq;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;

namespace IntelligentMachine.Twitch.IRC
{

    public class IntelliTwitchDataService
    {
        SQLiteConnection _connection;

        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #region Contructors

        public IntelliTwitchDataService(string databaseName)
        {

#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", databaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

            SendDebugMessage("FINAL DB PATH: " + dbPath);

            
        }

        #endregion

        #region Database Management

        public void CreateDB()
        {
            _connection.DropTable<IntelliTwitchUser>();
            _connection.DropTable<IntelliTwitchUserType>();

            _connection.CreateTable<IntelliTwitchUser>();
            _connection.CreateTable<IntelliTwitchUserType>();

            _connection.InsertAll(new[]{
                new IntelliTwitchUserType
            {
                UserType = Enums.IntelliTwitchUserTypes.empty,
                UserTypeName = "empty"
            },
                new IntelliTwitchUserType
            {
                UserType = Enums.IntelliTwitchUserTypes.mod,
                UserTypeName = "mod"

            },
                new IntelliTwitchUserType
            {
                UserType = Enums.IntelliTwitchUserTypes.global_mod,
                UserTypeName = "global_mod"
            },
                new IntelliTwitchUserType
            {
                UserType = Enums.IntelliTwitchUserTypes.admin,
                UserTypeName = "admin"
            },
                new IntelliTwitchUserType
            {
                UserType = Enums.IntelliTwitchUserTypes.staff,
                UserTypeName = "staff"
            }});
        }

        #region C.R.U.D.

        /// <summary>
        /// Will search for an existing user in the database.
        /// If a user is found, will determine if it needs an update.
        /// If no user is found, will create a new user in the database.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public IntelliTwitchUser GetUser(string[] userInfo)
        {
            //Should build if user doesnt exist, locate if it does. Located by ID
            for(var i = 0; i < userInfo.Length; i++)
            {
                if (userInfo[i].Contains("user-id"))
                {
                    var userSplit = userInfo[i].Split('=').Last();
                    int idCache;
                    if (Int32.TryParse(userSplit, out idCache))
                    {
                        if (UserExists(idCache))
                            return UpdateUser(_connection.Table<IntelliTwitchUser>().Where(x => x.ID == idCache).FirstOrDefault(), userInfo);
                        else
                            return CreateNewUser(userInfo, idCache);
                    }
                    else
                        SendDebugMessage("Couldn't parse UserID string to INT: " + userSplit[userSplit.Length - 1]);
                }
            }

            //For now we just return null. Might need to change this in the future. In theory the only time this runs is if we cant convert from string to int.
            return null;
        }

        /// <summary>
        /// Returns all users in the database
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IntelliTwitchUser> GetAllUsers()
        {
            return _connection.Table<IntelliTwitchUser>();
        }
        
        /// <summary>
        /// Creates a new user in the database. 
        /// Useful if you want to manually add a user and handle the IntelliTwitchUser creation yourself.
        /// Otherwise, GetUser() will automatically create/update a user if needed.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public int CreateUser(IntelliTwitchUser userInfo)
        {
            return _connection.Insert(userInfo);
        }

        /// <summary>
        /// Updates a user in the database.
        /// Compares the stored twitch user with the info recieved from twitch.
        /// Use GetUser() unless you want to handle this yourself.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public IntelliTwitchUser UpdateUser(IntelliTwitchUser user, string[] userInfo)
        {
            //Compare old user values, with new userInfo
            //Update if they are different
            var update = false;

            //TODO: Check if twitch user names can change easily, might be unneccessary
            if (user.UserName != IntelliTwitchStringParser.GetDisplayName(userInfo))
            {
                user.UserName = IntelliTwitchStringParser.GetDisplayName(userInfo);
                update = true;
            }
            if (user.UserNameColor != IntelliTwitchStringParser.GetUserNameColor(userInfo))
            {
                user.UserNameColor = IntelliTwitchStringParser.GetUserNameColor(userInfo);
                update = true;
            }
            if (user.IsSubscriber != IntelliTwitchStringParser.GetSubscriberStatus(userInfo))
            {
                user.IsSubscriber = IntelliTwitchStringParser.GetSubscriberStatus(userInfo);
                update = true;
            }
            if (user.IsTurbo != IntelliTwitchStringParser.GetTurboStatus(userInfo))
            {
                user.IsTurbo = IntelliTwitchStringParser.GetTurboStatus(userInfo);
                update = true;
            }
            if (user.IsChannelMod != IntelliTwitchStringParser.GetChannelModStatus(userInfo))
            {
                user.IsChannelMod = IntelliTwitchStringParser.GetChannelModStatus(userInfo);
                update = true;
            }

            if (update)
                _connection.Update(user);

            return user;
        }

        public int DeleteUser(IntelliTwitchUser user)
        {
            return _connection.Delete(user);
        }

        #endregion

        #region Helper Methods

        private IntelliTwitchUser CreateNewUser(string[] userInfo, int userID)
        {
            var user = new IntelliTwitchUser();
            //We already parsed out the user-id from the message, no need to relocate it. So we just pass it along.
            user.ID = userID;

            user.UserName = IntelliTwitchStringParser.GetDisplayName(userInfo);
            user.UserNameColor = IntelliTwitchStringParser.GetUserNameColor(userInfo);
            user.IsSubscriber = IntelliTwitchStringParser.GetSubscriberStatus(userInfo);
            user.IsTurbo = IntelliTwitchStringParser.GetTurboStatus(userInfo);
            user.IsChannelMod = IntelliTwitchStringParser.GetChannelModStatus(userInfo);
            user.UserTypeID = FindUserType(IntelliTwitchStringParser.GetUserType(userInfo));

            user.joinDate = GetCurrentUtcTime();

            CreateUser(user);

            return user;
        }

        private bool UserExists(int id)
        {
            var result = _connection.Table<IntelliTwitchUser>().Where(x => x.ID == id);
            return result.Count() > 0 ? true : false;
        }

        private int FindUserType(string msg)
        {
            switch (msg)
            {
                case "mod":
                    return GetUserTypePrimaryKey(Enums.IntelliTwitchUserTypes.mod);
                case "global_mod":
                    return GetUserTypePrimaryKey(Enums.IntelliTwitchUserTypes.global_mod);
                case "admin":
                    return GetUserTypePrimaryKey(Enums.IntelliTwitchUserTypes.admin);
                case "staff":
                    return GetUserTypePrimaryKey(Enums.IntelliTwitchUserTypes.staff);
                default:
                    return GetUserTypePrimaryKey(Enums.IntelliTwitchUserTypes.empty);
            }
        }

        private int GetCurrentUtcTime()
        {
            return (int)(DateTime.Now.ToUniversalTime() - epoch).TotalSeconds;
        }

        private int GetUserTypePrimaryKey(Enums.IntelliTwitchUserTypes userType)
        {
            var type = _connection.Table<IntelliTwitchUserType>().Where(x => x.UserType == userType).FirstOrDefault();

            return type.UserTypeID;
        }

        #endregion

        #endregion

        #region Debug

        private void SendDebugMessage(string msg)
        {
            Debug.Log(IntelliTwitchStringParser.BuildDebugMessage("DataService", msg));
        }

        #endregion
    }
}