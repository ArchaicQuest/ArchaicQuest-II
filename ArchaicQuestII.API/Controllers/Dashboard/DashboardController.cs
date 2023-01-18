using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Item;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.World.Room;
using ArchaicQuestII.GameLogic.Account;
using System.Globalization;
using ArchaicQuestII.API.Helpers;
using ArchaicQuestII.GameLogic.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ArchaicQuestII.Controllers.Dashboard
{
    public class QuickStats
    {
        public int ItemCount { get; set; }
        public int MobCount { get; set; }
        public int AreaCount { get; set; }
        public int RoomCount { get; set; }
        public int QuestCount { get; set; }
    }

    public class LineGraphStats
    {
        public List<LineStats> Data { get; set; } = new List<LineStats>();
    }

    public class LineStats
    {
        public string Name { get; set; }
        public List<Series> Series { get; set; } = new List<Series>();
    }

    public class Series
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDataBase _db;
        private readonly IPlayerDataBase _pdb;
        private readonly ICharacterHandler _characterHandler;
        public DashboardController(IDataBase db, IPlayerDataBase pdb, ICharacterHandler characterHandler)
        {
            _db = db;
            _pdb = pdb;
            _characterHandler = characterHandler;
        }

        [HttpGet]
        [Route("api/dashboard/quickStats")]
        public JsonResult get()
        {
            var stats = new QuickStats
            {
                ItemCount = _db.GetCollection<Item>(DataBase.Collections.Items).Count(),
                MobCount = _db.GetCollection<Player>(DataBase.Collections.Mobs).Count(),
                AreaCount = _db.GetCollection<Area>(DataBase.Collections.Area).Count(),
                RoomCount = _db.GetCollection<Room>(DataBase.Collections.Room).Count(),
                //QuestCount = _db.GetCollection<QuestLog>(DataBase.Collections.Quests).Count()
            };

            return Json(stats);

        }


        [HttpGet]
        [Route("api/dashboard/who")]
        public JsonResult WhoList()
        {
            var playingPlayers = _characterHandler.GetPlayerCache().ToList();
            return Json(playingPlayers);

        }

        [HttpGet]
        [Route("api/dashboard/AccountStats")]
        public JsonResult AccountStats()
        {


            var lineGraphStats = new LineGraphStats();
            var AccountStats = new LineStats()
            {
                Name = "Accounts"
            };



            var accounts = _pdb.GetCollection<Account>(PlayerDataBase.Collections.Account).FindAll();


            var thisMonth = accounts.Where(X => X.DateJoined.Month.Equals(DateTime.Now.Month)).Count();
            var data = new Series();
            data.Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Year;
            data.Value = thisMonth;
            AccountStats.Series.Add(data);


            var months = 6;

            for (int i = 1; i < months; i++)
            {
                var LastMonth = accounts.Where(X => X.DateJoined.Month.Equals(DateTime.Now.AddMonths(-i).Month)).Count();
                var LastMonthData = new Series
                {
                    Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.AddMonths(-i).Month) + " " + DateTime.Now.AddMonths(-i).Year,
                    Value = LastMonth
                };

                AccountStats.Series.Add(LastMonthData);
            }

            //Characters

            var CharStats = new LineStats()
            {
                Name = "Characters"
            };

            var players = _pdb.GetCollection<Player>(PlayerDataBase.Collections.Players).FindAll();





            var CharThisMonth = players.Where(X => X.JoinedDate.Month.Equals(DateTime.Now.Month)).Count();
            var CharData = new Series();
            CharData.Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Year;
            CharData.Value = CharThisMonth;
            CharStats.Series.Add(CharData);

            for (int i = 1; i < months; i++)
            {
                var LastMonth = players.Where(X => X.JoinedDate.Month.Equals(DateTime.Now.AddMonths(-i).Month)).Count();
                var LastMonthData = new Series
                {
                    Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.AddMonths(-i).Month) + " " + DateTime.Now.AddMonths(-i).Year,
                    Value = LastMonth
                };

                CharStats.Series.Add(LastMonthData);
            }


            lineGraphStats.Data.Add(AccountStats);
            lineGraphStats.Data.Add(CharStats);

            return Json(lineGraphStats);

        }

        [HttpGet]
        [Route("api/dashboard/Logins")]
        public JsonResult LoginStats()
        {


            var lineGraphStats = new LineGraphStats();
            var AccountStats = new LineStats()
            {
                Name = "Logins"
            };



            var accounts = _pdb.GetCollection<AccountLoginStats>(PlayerDataBase.Collections.LoginStats).FindAll();


            var today = accounts.Where(X => X.loginDate.Date.Equals(DateTime.Now.Date)).Count();
            var data = new Series();
            data.Name = DateTime.Now.ToString("dd-MM-yy");
            data.Value = today;
            AccountStats.Series.Add(data);


            var days = 30;

            for (int i = 1; i < days; i++)
            {
                var yesterday = accounts.Where(X => X.loginDate.Date.Equals(DateTime.Today.AddDays(-i).Date)).Count();
                var yesterdayData = new Series
                {
                    Name = DateTime.Today.AddDays(-i).ToString("dd-MM-yy"),
                    Value = yesterday
                };

                AccountStats.Series.Add(yesterdayData);
            }

            var UniqueLoginStats = new LineStats()
            {
                Name = "Unique Logins"
            };


            var accountLogins = accounts.Where(X => X.loginDate.Equals(DateTime.Now.Date));
            var uniqueList = new List<AccountLoginStats>();

            foreach (var login in accounts)
            {

                if (uniqueList.FirstOrDefault(x => x.AccountId == login.AccountId && x.loginDate.Date == login.loginDate.Date) == null)
                {
                    uniqueList.Add(login);
                }

            }

            var UniqueToday = uniqueList.Where(x => x.loginDate.Date == DateTime.Now.Date).Count();
            var uniqueData = new Series();
            uniqueData.Name = DateTime.Now.ToString("dd-MM-yy");
            uniqueData.Value = UniqueToday;
            UniqueLoginStats.Series.Add(uniqueData);


            for (int i = 1; i < days; i++)
            {
                var yesterday = uniqueList.Where(X => X.loginDate.Date.Equals(DateTime.Today.AddDays(-i).Date)).Count();
                var yesterdayData = new Series
                {
                    Name = DateTime.Today.AddDays(-i).ToString("dd-MM-yy"),
                    Value = yesterday
                };

                UniqueLoginStats.Series.Add(yesterdayData);
            }



            lineGraphStats.Data.Add(AccountStats);
            lineGraphStats.Data.Add(UniqueLoginStats);

            return Json(lineGraphStats);

        }

        [HttpGet]
        [Route("api/dashboard/MobKills")]
        public JsonResult MobKillStats()
        {


            var lineGraphStats = new LineGraphStats();
            var AccountStats = new LineStats()
            {
                Name = "Mob Kills"
            };



            var accounts = _pdb.GetCollection<MobStats>(PlayerDataBase.Collections.MobStats).FindAll();

            var getKills = accounts.FirstOrDefault(X => X.Date.Date.Equals(DateTime.Now.Date));

            var today = getKills == null ? 0 : getKills.MobKills;
            var data = new Series();
            data.Name = DateTime.Now.ToString("dd-MM-yy");
            data.Value = today;
            AccountStats.Series.Add(data);


            var days = 30;

            for (int i = 1; i < days; i++)
            {
                var yesterday = accounts.FirstOrDefault(X => X.Date.Date.Equals(DateTime.Today.AddDays(-i).Date));
                var yesterDayKills = yesterday == null ? 0 : yesterday.MobKills;

                var yesterdayData = new Series
                {
                    Name = DateTime.Today.AddDays(-i).ToString("dd-MM-yy"),
                    Value = yesterDayKills
                };

                AccountStats.Series.Add(yesterdayData);
            }

            var playerDeaths = new LineStats()
            {
                Name = "Player Deaths"
            };


            var accountLogins = accounts.FirstOrDefault(X => X.Date.Date.Equals(DateTime.Now.Date));
            var getPlayerDeaths = accountLogins == null ? 0 : accountLogins.PlayerDeaths;


            var UniqueToday = getPlayerDeaths;
            var uniqueData = new Series();
            uniqueData.Name = DateTime.Now.ToString("dd-MM-yy");
            uniqueData.Value = UniqueToday;
            playerDeaths.Series.Add(uniqueData);


            for (int i = 1; i < days; i++)
            {
                var yesterday = accounts.FirstOrDefault(X => X.Date.Date.Equals(DateTime.Today.AddDays(-i).Date));
                var yesterDayKills = yesterday == null ? 0 : yesterday.PlayerDeaths;
                var yesterdayData = new Series
                {
                    Name = DateTime.Today.AddDays(-i).ToString("dd-MM-yy"),
                    Value = yesterDayKills
                };

                playerDeaths.Series.Add(yesterdayData);
            }



            lineGraphStats.Data.Add(AccountStats);
            lineGraphStats.Data.Add(playerDeaths);

            return Json(lineGraphStats);

        }

    }
}
