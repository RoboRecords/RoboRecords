/*
 * Methods.cs
 * Copyright (C) 2022, Zenya <Zeritar> and Refrag <Refragg>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * See the 'LICENSE' file for more details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestConsole.Models;
using RoboRecords;
using RoboRecords.Models;
using RoboRecords.DbInteraction;

namespace TestConsole
{
    public enum MenuAction
    {
        Quit,
        WriteLine,
        CallMenu,
        SelectRoboGames,
        TestUpdate,
        TestRecord,
        TryReadPK3,
        TryAddCyberdime,
        TryAddRedVolcano2,
        TryAddZenyaTheModerator,
        TryTestFileManager,
    }
    class Methods
    {

        public void DrawMenu(Menu menu)
        {
            // Console.Clear();

            Console.WriteLine(menu.name + "\n");
            for (int i = 0; i < menu.menuItems.Count; i++)
            {
                Console.WriteLine($"({i + 1}) {menu.menuItems[i].name}");
            }
            GetMenuAction(menu);
        }

        public void GetMenuAction(Menu menu)
        {
            bool validChoice = false;
            int yPos = 0;

            while (!validChoice)
            {
                Console.WriteLine("\nEnter your choice:");
                yPos = Console.GetCursorPosition().Top;
                int choice;
                validChoice = int.TryParse(Console.ReadLine(), out choice);
                if (!validChoice || choice < 1 || choice > menu.menuItems.Count)
                    validChoice = false;
                else
                {
                    DoMenuAction(menu.menuItems[choice - 1]);
                }
            }
        }

        // Zenya's methods to test if database interactions work properly. These are not supposed to be called.
        //DbInteraction.DbTester.TestUpdate();
        //DbInteraction.DbTester.TestRecord(); // This will do nothing if TestUpdate isn't run first. This shouldn't cause a crash.
        //DbInteraction.DbTester.TryReadPK3(); // Reads the Cyberdime PK3 without adding it to the database.
        //DbInteraction.DbTester.TryAddCyberdime(); // Reads Cyberdime PK3 and adds Cyberdime Realm RoboGame to database.
        //DbInteraction.DbTester.TryAddRedVolcano2(); // Add Red Volcano Act 2 with automatic Level Group sorting so it appears next to Act 1.
        //DbInteraction.DbTester.TryAddZenyaTheModerator(); // Doesn't work currently

        public void DoMenuAction(MenuItem item)
        {
            if (item.parameter == null)
                item.parameter = "";

            switch (item.action)
            {
                case MenuAction.WriteLine:
                    Console.WriteLine(item.parameter);
                    Console.ReadKey();
                    break;
                case MenuAction.CallMenu:
                    Console.WriteLine("New menu goes here");
                    Console.ReadKey();
                    break;
                case MenuAction.SelectRoboGames:
                    SelectRoboGames();
                    Console.ReadKey();
                    break;
                case MenuAction.TestUpdate:
                    DoTestUpdate();
                    Console.ReadKey();
                    break;
                case MenuAction.TestRecord:
                    DoTestRecord();
                    Console.ReadKey();
                    break;
                case MenuAction.TryReadPK3:
                    DoTryReadPK3();
                    Console.ReadKey();
                    break;
                case MenuAction.TryAddCyberdime:
                    DoTryAddCyberdime();
                    Console.ReadKey();
                    break;
                case MenuAction.TryAddRedVolcano2:
                    DoTryAddRedVolcano2();
                    Console.ReadKey();
                    break;
                case MenuAction.TryAddZenyaTheModerator:
                    DoTryAddZenyaTheModerator();
                    Console.ReadKey();
                    break;
                case MenuAction.TryTestFileManager:
                    DoTestFileManager();
                    Console.ReadKey();
                    break;
                case MenuAction.Quit:
                    DoQuit();
                    break;
            }
        }

        public void DoQuit()
        {
            Environment.Exit(0);
        }

        public void DoTestUpdate()
        {
            DbTester.TestUpdate();
            bool foundLevel = DbSelector.TryGetGameLevelFromMapId("sonicroboblast2v22", "17", out RoboLevel testLevel);

            if (foundLevel)
            {
                Console.Clear();
                Console.WriteLine("TestUpdate succeeded.\nResult entry:");
                int yPos = Console.GetCursorPosition().Top;

                Console.SetCursorPosition(0, yPos);
                Console.Write("Id");
                Console.SetCursorPosition(5, yPos);
                Console.Write("Level name");
                Console.SetCursorPosition(30, yPos);
                Console.Write("Act");
                Console.SetCursorPosition(35, yPos);
                Console.Write("Map");

                yPos++;

                Console.SetCursorPosition(0, yPos);
                Console.Write(testLevel.DbId);
                Console.SetCursorPosition(5, yPos);
                Console.Write(testLevel.LevelName);
                Console.SetCursorPosition(30, yPos);
                Console.Write(testLevel.Act);
                Console.SetCursorPosition(35, yPos);
                Console.Write(testLevel.LevelNumber);

                Console.WriteLine("\nPress any key to undo changes and return to the menu.");
                Console.ReadKey();

                DbDeleter.DeleteRoboLevel(testLevel);
            }
            else
            {
                Console.WriteLine("Test failed.");
                Console.WriteLine("\nPress any key to return to the menu.");
                Console.ReadKey();
            }
        }

        public void DoTestRecord()
        {
            DbTester.TestRecord();
            DbSelector.TryGetGameLevelFromMapId("sonicroboblast2v22", "16", out RoboLevel testLevel);
            RoboRecord testRecord = testLevel.Records.Where(
                e => e.Uploader.UserNameNoDiscrim == "ZeriTAS" &&
                e.Tics == 9100 &&
                e.Character.NameId == "amy").FirstOrDefault();

            if (testRecord is not null)
            {
                Console.Clear();
                Console.WriteLine("TestRecord succeeded.\nResult entry:");
                int yPos = Console.GetCursorPosition().Top;

                Console.SetCursorPosition(0, yPos);
                Console.Write("Id");
                Console.SetCursorPosition(5, yPos);
                Console.Write("Level");
                Console.SetCursorPosition(30, yPos);
                Console.Write("Time");
                Console.SetCursorPosition(40, yPos);
                Console.Write("Char");
                Console.SetCursorPosition(55, yPos);
                Console.Write("Uploader");

                yPos++;

                Console.SetCursorPosition(0, yPos);
                Console.Write(testRecord.DbId);
                Console.SetCursorPosition(5, yPos);
                if (testLevel.Act > 0)
                    Console.Write($"{testLevel.LevelName} Act {testLevel.Act}");
                else
                    Console.Write(testLevel.LevelName);
                Console.SetCursorPosition(30, yPos);
                Console.Write(RoboRecord.GetTimeFromTics(testRecord.Tics));
                Console.SetCursorPosition(40, yPos);
                Console.Write(testRecord.Character.Name);
                Console.SetCursorPosition(55, yPos);
                Console.Write(testRecord.Uploader.UserNameNoDiscrim);

                Console.WriteLine("\nPress any key to undo changes and return to the menu.");
                Console.ReadKey();

                DbDeleter.DeleteRoboRecord(testRecord);
            }
            else
            {
                Console.WriteLine("Test failed.");
                Console.WriteLine("\nPress any key to return to the menu.");
                Console.ReadKey();
            }
        }

        public void DoTryReadPK3()
        {
            Console.Clear();
            DbTester.TryReadPK3();
            Console.WriteLine("\nEnd of WadReader. If there's stuff above here, then test was successful.");

            Console.WriteLine("\nPress any key to return to the menu.");
            Console.ReadKey();
        }

        public void DoTryAddCyberdime()
        {
            Console.Clear();
            DbTester.TryAddCyberdime();

            bool foundGame = DbSelector.TryGetGameWithRecordsFromID("cyber", out RoboGame testGame);

            if (foundGame)
            {
                Console.WriteLine("TryAdCyberdime succeeded.\nResult:\n");
                Console.WriteLine(testGame.Name + "\n");

                int yPos = Console.GetCursorPosition().Top;

                Console.SetCursorPosition(0, yPos);
                Console.Write("Id");
                Console.SetCursorPosition(5, yPos);
                Console.Write("Level name");
                Console.SetCursorPosition(30, yPos);
                Console.Write("Act");
                Console.SetCursorPosition(35, yPos);
                Console.Write("Map");

                if (testGame.LevelGroups.Count > 0)
                    foreach (LevelGroup group in testGame.LevelGroups)
                    {
                        if (group.Levels.Count > 0)
                            foreach (RoboLevel testLevel in group.Levels)
                            {
                                yPos++;

                                Console.SetCursorPosition(0, yPos);
                                Console.Write(testLevel.DbId);
                                Console.SetCursorPosition(5, yPos);
                                Console.Write(testLevel.LevelName);
                                Console.SetCursorPosition(30, yPos);
                                Console.Write(testLevel.Act);
                                Console.SetCursorPosition(35, yPos);
                                Console.Write(testLevel.LevelNumber);
                            }
                    }

                if (testGame.LevelGroups.Count > 0)
                    foreach (LevelGroup group in testGame.LevelGroups)
                    {
                        if (group.Levels.Count > 0)
                            foreach (RoboLevel testLevel in group.Levels)
                            {
                                DbDeleter.DeleteRoboLevel(testLevel);
                            }
                    }

                Console.WriteLine("\nPress any key to undo changes and return to the menu.");
                Console.ReadKey();

                DbDeleter.DeleteRoboGame(testGame);
            }
            else
            {
                Console.WriteLine("Test failed.");
                Console.WriteLine("\nPress any key to return to the menu.");
                Console.ReadKey();
            }
        }

        public void DoTryAddRedVolcano2()
        {
            DbTester.TryAddRedVolcano2();
            bool foundLevel = DbSelector.TryGetGameLevelFromMapId("sonicroboblast2v22", "17", out RoboLevel testLevel);

            if (foundLevel)
            {
                Console.Clear();
                Console.WriteLine("TestUpdate succeeded.\nResult entry:");
                int yPos = Console.GetCursorPosition().Top;

                Console.SetCursorPosition(0, yPos);
                Console.Write("Id");
                Console.SetCursorPosition(5, yPos);
                Console.Write("Level name");
                Console.SetCursorPosition(30, yPos);
                Console.Write("Act");
                Console.SetCursorPosition(35, yPos);
                Console.Write("Map");

                yPos++;

                Console.SetCursorPosition(0, yPos);
                Console.Write(testLevel.DbId);
                Console.SetCursorPosition(5, yPos);
                Console.Write(testLevel.LevelName);
                Console.SetCursorPosition(30, yPos);
                Console.Write(testLevel.Act);
                Console.SetCursorPosition(35, yPos);
                Console.Write(testLevel.LevelNumber);

                Console.WriteLine("\nPress any key to undo changes and return to the menu.");
                Console.ReadKey();

                DbDeleter.DeleteRoboLevel(testLevel);
            }
            else
            {
                Console.WriteLine("Test failed.");
                Console.WriteLine("\nPress any key to return to the menu.");
                Console.ReadKey();
            }
        }

        public void DoTryAddZenyaTheModerator()
        {
            DbTester.TryAddZenyaTheModerator();
            Console.WriteLine("\nPress any key to return to the menu.");
            Console.ReadKey();
        }

        public void DoTestFileManager()
        {
            Console.WriteLine("Enter filename:");
            string filename = Console.ReadLine();
            Console.WriteLine($"\nTrying to create {filename} at {"EnvVars.SftpHost"}/RoboRecords\n");
            FileManager.Initialize();
            FileManager.CreateFile(filename);
        }

        public void SelectRoboGames()
        {
            DbSelector.TryGetGames(out List<RoboGame> games);

            Console.Clear();
            Console.WriteLine("Query Result:\n");
            for (int i = 0; i < games.Count; i++)
            {
                Console.WriteLine($"  {i + 1}  {games[i].Name}");
            }
        }
    }
}
