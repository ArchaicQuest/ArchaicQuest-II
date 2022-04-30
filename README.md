# ArchaicQuest II - MUD codebase written in C#
![alt ArchaicQuest II](https://i.imgur.com/LUv3vGm.png)

## A MUD codebase to create a multiplayer text based RPG, also known a Multi User Dungeon (MUD)

Goal of ArchaicQuest II is to make a MUD that feels nostalgic and compelling to play as well as providing a MUD codebase that makes building and managing your own game simple and fun especially for non coders with the help of the admin tool.

[play.archaicquest.com](https://play.archaicquest.com) is the flagship MUD built with ArchaicQuest II.

## Current Features
- Currently 54 commands, commands can also be abbreviated  🔠
- Cardinal and Ordinal Movement directions including up and down 🦶
- Auto Attack combat rounds ⚔
- Skills & spells 💫
- 350+ socials 😃 
- Look, examine, smell, taste, and touch objects 👁🔎👃👅🤏
- Day and Night cycles 🌞🌛
- Communication among players publicly and privately 💬
- NPCs can follow waypoints and execute commands 🎭
- Ability to add simple or complex quests with Lua Scripting ⁉
- Event scripts for NPCs/Rooms for your scripting needs 📜
- Room and NPC emotes 💃

## About the Projects

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
Use `dotnet run --project ArchaicQuestII.API/ArchaicQuestII.API.csproj`.

If on unix you can use the make file in the project to run using `make run`. 

For Windows using visual studio just hit the run button.

There's no output to say it's running but once you make a request it wakes up and responds.

### Basic Structure overview

- ArchaicQuestII.API
  - Handles all the API endpoints and initialises everything in Startup.cs
- ArchaicQuestII.DataAccess
  - Wrapper around LiteDB. To add new collections you need to modify Collections enum and the GetCollectionName switch
- ArchaicQuestII.GameLogic
  - All game logic here plus the SignalR and Telnet logic. Best place to start is Commands.cs to see what commands exists and how to add more.
 
### Adding Commands / Features

View the existing structure in Commands.cs there may be an interface where you can add your new command. If not follow the existing structure to add a new interface and class and make sure to update the tests once you add the new interface to Commands.cs 

To add new skills and spells use the web admin tool, skills and spells can be scripted with Lua for custom formulas and effects.

Majority of features added have been built with the admin tool in mind so most content can be added or modified without coding or making a deployment

### Telnet Support
Traditionally MUDs use telnet as the way to communicate but with ArchaicQuest II it's web only, the browser can offer a richer, consistent, and streamlined interface for all players without having them to download a client. 

### Want to get involved?
If you're a coder, writer, MUD player, or someone who wants to help up then get in touch and say 'Hello' on the discord server: [https://discordapp.com/invite/nuf7FVq](https://discordapp.com/invite/nuf7FVq)

---

## Admin  Tool
View the [ArchaicQuest II - Admin tool](https://github.com/ArchaicQuest/ArchaicQuest-II-Web-Admin) to find out more.

![alt ArchaicQuestII Web Admin](https://cdn.discordapp.com/attachments/660365544377155604/764419912088420352/editRoom.PNG)

## Game Client
View the [ArchaicQuest II - Game Client](https://github.com/ArchaicQuest/ArchaicQuest-II-Web-Client) to find out more.

![alt ArchaicQuestII Web Admin](https://cdn.discordapp.com/attachments/660365544377155604/764419914970300456/web_client.PNG)
