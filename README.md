# ArchaicQuest II - MUD codebase written in C#
![alt ArchaicQuest II](https://i.imgur.com/LUv3vGm.png)

## A MUD codebase to create a multiplayer text based RPG, also known a Multi User Dungeon (MUD)

ArchaicQuest II comprises of [3 projects](https://github.com/ArchaicQuest) that are required together.


| Project                                                                                     | Description                                                                                                            |
| ------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------- |
| [ArchaicQuest II](https://github.com/ArchaicQuest/ArchaicQuest-II)                          | C# game engine, contains the web API for the admin tool and the SignalR hub for the web socket connects to the client. |
| [ArchaicQuest II - Admin tool](https://github.com/ArchaicQuest/ArchaicQuest-II-Web-Admin)   | Angular 8+ web admin, allows creation and management of your MUD world.                                                |
| [ArchaicQuest II - Game Client](https://github.com/ArchaicQuest/ArchaicQuest-II-Web-Client) | Angular 8+ web client for connecting to the game and playing with others.                                              |

## ArchaicQuest II

The C# project is .NET Core 3.1 and uses [LiteDb](https://www.litedb.org/) which is an embedded NoSQL database. Within Startup.cs it will seed the database on first run with the necessary defaults that are required. 

* Alignments
* AttackTypes
* Skills
* Races
* Status
* Classes
* Config

Some of the above can be edited in the admin tool, the rest if required to be changed needs to be done in code.

The last important step that isn't seeded yet is the very first room! For now before connecting to the game you will need to fire up the web admin tool and create an area and then create a room in said area. Then the web client will be able to connect correctly. 

### Running the project
Use `dotnet run -p ArchaicQuestII.API/ArchaicQuestII.API.csproj`.

If on unix you can use the make file in the project to run using `make run`. 

For Windows using visual studio just hit the run button.

There's no output to say it's running but once you make a request it wakes up and responds.

### Basic Structure overview

- ArchaicQuestII.API
  - Handles all the API endpoints and initialises everything in Startup.cs
- ArchaicQuestII.DataAccess
  - Wrapper around LiteDB. To add new collections you need to modify Collections enum and the GetCollectionName switch
- ArchaicQuestII.GameLogic
  - All game logic here plus the Signalr and Telnet logic. Best place to start is Commands.cs to see what commands exists and how to add more.
 
### Adding Commands / Features

View the existing structure in Commands.cs there may be an interface where you can add your new command. If not follow the existing structure to add a new interface and class and make sure to update the tests once you add the new interface to Commands.cs 

To add new skills and spells use the web admin tool, skills and spells can be scripted with Lua for custom formulas and effects.

Majority of features added have been built with the admin tool in mind so most content can be added or modified without coding or making a deployment

### Current Features
- Currently 54 commands, commands can also be abbreviated  🔠
- Cardinal and Ordinal Movement directions including up and down 🦶
- Auto Attack combat ⚔
- Skills & spells 💫
- 350+ socials 😃 
- Look, examine, smell, taste, and touch 👁🔎👃👅🤏
- Day and Night cycles 🌞🌛
- Communication among players publicly and privately 💬
- NPCs can follow waypoints and execute commands 🎭
- Ability to add simple or complex quests with Lua Scripting ⁉
- Event scripts for NPCs/Rooms for your scripting needs 📜
  
