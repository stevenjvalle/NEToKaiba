# NamazuKingdomBot

A bot built in Discord.NET using EF Core, SQLite, and NAudio.

The primary purpose of this bot is to provide a TTS for people who do not want to speak in a voice chat but still wish to contribute to a conversation. 

To be able to use the bot you need to do a few things.
1) Copy optus.dll, libsodium.dll, and bot.db into the location of the executable; i.e. \NamazuKingdomBot\NamazuKingdom\bin\Debug\net6.0
2) Add a folder named "sounds" into the same directory; this is where you can add mp3 files to play
3) Create an appsettings.json, and follow the example appsettings.json.example in the same directory as above