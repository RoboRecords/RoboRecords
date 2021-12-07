using RoboRecords.DatabaseContexts;
using System;
using TestConsole.Models;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TestConsole
{
    // Zenya sucks at Unit Tests
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
        static void Main(string[] args)
        {
            Configure();

            Methods methods = new Methods();

            Menu menu = new Menu("Main Menu");
            MenuItem testItem = new MenuItem("Select all RoboGames", MenuAction.SelectRoboGames);
            testItem.parameter = $"{testItem.name} menu action called.";
            menu.menuItems.Add(testItem);

            testItem = new MenuItem("TestUpdate", MenuAction.TestUpdate);
            testItem.parameter = $"{testItem.name} menu action called.";
            menu.menuItems.Add(testItem);

            testItem = new MenuItem("TestRecord", MenuAction.TestRecord);
            testItem.parameter = $"{testItem.name} menu action called.";
            menu.menuItems.Add(testItem);

            testItem = new MenuItem("TryReadPK3", MenuAction.TryReadPK3);
            testItem.parameter = $"{testItem.name} menu action called.";
            menu.menuItems.Add(testItem);

            testItem = new MenuItem("TryAddCyberdime", MenuAction.TryAddCyberdime);
            testItem.parameter = $"{testItem.name} menu action called.";
            menu.menuItems.Add(testItem);

            testItem = new MenuItem("TryAddRedVolcano2", MenuAction.TryAddRedVolcano2);
            testItem.parameter = $"{testItem.name} menu action called.";
            menu.menuItems.Add(testItem);

            testItem = new MenuItem("Exit", MenuAction.Quit);
            testItem.parameter = $"";
            menu.menuItems.Add(testItem);

            while (true)
                methods.DrawMenu(menu);
        }



        static void Configure()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<Program>();

            Configuration = builder.Build();

            RoboRecordsDbContext.SetConnectionString(Configuration["TempSqlConnectionString"]);
            IdentityContext.SetConnectionString(Configuration["TempUserConnectionString"]);
        }
    }
}