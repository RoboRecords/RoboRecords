using System.Collections.Generic;
using System;
using System.Web;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using RoboRecords.DatabaseContexts;
using RoboRecords.Models;
using System.Linq;

namespace RoboRecords.DbInteraction
{
    public class DbSelector
    {
        public static List<RoboGame> GetAllGameData()
        {
            List<RoboGame> _roboGames = new List<RoboGame>();

            // SELECT * FROM RoboGames, JOIN all foreign keys
            using (RoboRecordsDbContext context = new RoboRecordsDbContext())
            {
                _roboGames = context.RoboGames
                .Include(e => e.LevelGroups)
                .ThenInclude(levelGroups => levelGroups.Levels)
                .ThenInclude(levels => levels.Records)
                .ThenInclude(records => records.Character)
                .Include(e => e.LevelGroups)
                .ThenInclude(levelGroups => levelGroups.Levels)
                .ThenInclude(levels => levels.Records)
                .ThenInclude(records => records.Uploader)
                .ToListAsync().Result;
            }


            // Sort the levels by level number, as they may not be in order in the database
            if (_roboGames.Count > 0)
                foreach (var roboGame in _roboGames)
                {
                    if (roboGame.LevelGroups.Count > 0)
                        foreach (var levelGroup in roboGame.LevelGroups)
                        {
                            List<RoboLevel> sortedList = levelGroup.Levels.OrderBy(l => l.LevelNumber).ToList();
                            levelGroup.Levels = sortedList;
                        }
                }

            return _roboGames;
        }

        public static List<RoboGame> GetGames()
        {
            List<RoboGame> _roboGames = new List<RoboGame>();

            // SELECT * FROM RoboGames, no JOINs. Used for Games page.
            using (RoboRecordsDbContext context = new RoboRecordsDbContext())
            {
                _roboGames = context.RoboGames
                .ToListAsync().Result;
            }

            return _roboGames;
        }

        public static RoboGame GetGameFromID(string id)
        {
            RoboGame _roboGame;
            // SELECT * FROM RoboGames WHERE UrlName = id
            using (RoboRecordsDbContext context = new RoboRecordsDbContext())
            {
                _roboGame = context.RoboGames.Where(e => e.UrlName == id).FirstOrDefault();
            }

            return _roboGame;
        }

        public static RoboLevel GetGameLevelFromMapId(string gameid, string _mapid)
        {
            // SELECT * FROM RoboLevels where gameid and mapid --- Took 6 hours to figure this one out; Zenya
            using (RoboRecordsDbContext context = new RoboRecordsDbContext())
            {
                int mapid = 0;
                int.TryParse(_mapid, out mapid);

                RoboLevel query = context.RoboGames
                    .Include(g => g.LevelGroups)
                    .ThenInclude(l => l.Levels)
                    .ThenInclude(l => l.Records)
                    .ThenInclude(r => r.Character)
                    .Include(g => g.LevelGroups)
                    .ThenInclude(l => l.Levels)
                    .ThenInclude(l => l.Records)
                    .ThenInclude(r => r.Uploader)
                    .Where(g => g.UrlName == gameid && g.LevelGroups.Any(l => l.Levels.Any(level => level.LevelNumber == mapid)))
                    .Select(g => g.LevelGroups.Where(e => e.Levels.Any(o => o.LevelNumber == mapid))
                    .Select(e => e.Levels.FirstOrDefault(l => l.LevelNumber == mapid))
                    .FirstOrDefault())
                    .FirstOrDefault();

                if (query != null)
                    return query;
                else
                    return new RoboLevel()
                    {
                        DbId = -1,
                        LevelName = "Invalid Level",
                        LevelNumber = 9001,
                        Act = 0,
                        IconUrl = "",
                        Records = new List<RoboRecord>()
                    };
            }
        }

        public static List<RoboGame> GetGamesWithLevels()
        {
            List<RoboGame> _roboGames = new List<RoboGame>();

            // SELECT * FROM RoboGames, JOIN levels
            using (RoboRecordsDbContext context = new RoboRecordsDbContext())
            {
                _roboGames = context.RoboGames
                .Include(e => e.LevelGroups)
                .ThenInclude(levelGroups => levelGroups.Levels)
                .ToListAsync().Result;
            }


            // Sort the levels by level number, as they may not be in order in the database
            if (_roboGames.Count > 0)
                foreach (var roboGame in _roboGames)
                {
                    if (roboGame.LevelGroups.Count > 0)
                        foreach (var levelGroup in roboGame.LevelGroups)
                        {
                            List<RoboLevel> sortedList = levelGroup.Levels.OrderBy(l => l.LevelNumber).ToList();
                            levelGroup.Levels = sortedList;
                        }
                }

            return _roboGames;
        }
    }
}