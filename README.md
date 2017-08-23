# IntelliTwitch
## What does it do?
IntelliTwitch is a Twitch Chat bot built for Unity3D. I designed it to use as a framework for making a "Twitch Plays" style game.
However, It can also be used for any purpose that requires listening to Twitch chat via IRC. IntelliTwitch has support for remembering
user information from chat via SQLite4Unity3D(see acknowledgements), and can be expanded to work with whatever else you would like to 
store in the database.

## Why use IntelliTwitch?

IntelliTwitch tries to be user friendly. You can easily create commands that can access any part of your game by inherriting from
 the `IIntelliTwitchCommand` interface, and the `IntelliTwitchCommandAction` class. See below for more about using IntelliTwitch.
 IntelliTwitch will find all of the created commands, and actions using reflection during startup. IntelliTwitch also attempts to
 be thread-safe by utilizing `RingBuffer` (see acknowledgements).
 
 ## Usage
 - Attach `IntelliTwitchManager` to a `GameObject`.
 - Right click, or use the Create Asset menu, and navigate to `IntelliTwitch > Bot Info` to create a new `IntelliTwitchBotInfo`
 file to place
 in the `IntelliTwitchManager`.
 - Log into Twitch on your browser on the account the bot will use, and navigate to [this tool](https://twitchapps.com/tmi/) to
 generate your oAuth code. Place you oAuth code inside of the `GlobalSettings` class.
 - Add the channels you would like to connect to, and input your bot's username into the `BotInfo` file you created.
 username case does not matter.
 - Attach your `IntelliTwitchBotInfo` to the `IntelliTwitchManager` field labeled `Bot Info`.
 - Give your database a name, without the `.db` extension, within the `IntelliTwitchManager`.
 - **The `Streaming Assets` folder is 100% necessary even though it starts empty. Your database file will appear here after this first run.
 
 
 ## Creating Commands and Actions
 
 - `IIntelliTwitchCommand` - Defines what the bot will respond to. Your command must include the ! , see the example commands for more.
 - `IntelliTwitchCommandAction` - Defines how the bot will react to the command, refer to the example actions for a barebones example.
 
 ## Breakdown of IntelliTwitch's Parts
 
 - `IntelliTwitchManager` - This will be your main access point to it from your game, it always runs on the main thread.
 - `IntelliTwitchIRC` - This handles the connection between your game and Twitch IRC. It is the heart of the program.
 It does **NOT** run on the main thread.
 - `IntelliTwitchInterpreter` - This handles converting Twitch chat from nasty strings, to a more user friendly format. It is essentially
 the translator. Commands are processed here, and sent back to `IntelliTwitchIRC` as `IntelliTwitchCommandAction`'s.
 - `IntelliTwitchDataService` - This is where all of the database interactions are handled. Currently only handles CRUD operations
 on the `IntelliTwitchUser` database. Easily expandable if you need more. See SQLite4Unity3D in the acknowledgements section for help
 on customizing the database to fit your needs.
 - `IntelliTwitchStringParser` - This is a helper class that will pull information from the Twitch Chat message, provided you have run
 `string.Split(';')` on it beforehand. Useful if you would like to customize it to fit a certain need. Otherwise, it is designed
 to work automagically with the `IntelliTwitchInterpreter`.
 
 ## Acknowledgements
 - SQLite4Unity3D - https://github.com/codecoding/SQLite4Unity3d (license found in main license file)
 - Disruptor-unity3d - https://github.com/dave-hillier/disruptor-unity3d (license found inside `disruptor-unity3d` folder)
 
 Proper licensing is included with this project for both. 
