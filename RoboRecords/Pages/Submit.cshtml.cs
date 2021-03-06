/*
 * Submit.cshtml.cs: Backend for the replay submission page
 * Copyright (C) 2022, Ors <Riku-S>, Zenya <Zeritar> and Refrag <Refragg>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * See the 'LICENSE' file for more details.
 */

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using RoboRecords.DatabaseContexts;
using RoboRecords.DbInteraction;
using RoboRecords.Models;

namespace RoboRecords.Pages
{
    public class Submit : RoboPageModel
    {
        public static List<RoboRecord> RecordList;
        [BindProperty]
        public ReplayUploadDb FileUpload { get; set; }
        public static RoboGame Game;
        public static bool UserLoggedIn;

        public Submit(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        public class ErrorMessage
        {
            public string ReplayName;
            public string Message;

            public ErrorMessage(string replayName, string message)
            {
                ReplayName = replayName;
                Message = message;
            }
        }

        public static List<ErrorMessage> ErrorMessages;
        public void OnGet()
        {
            RecordList = new List<RoboRecord>();
            ErrorMessages = new List<ErrorMessage>();
            // TODO: Change to only select current level, as this won't make Entity Framework
            // generate an UPDATE for the whole RoboGame object.
            // Not gonna start changing stuff here, as it might break the Submit page.
            // Same method as in Map page can be used to get just the level.
            // DbInserter.AddRecord just
            DbSelector.TryGetGamesWithLevels(out var roboGames);

            var gameId = HttpUtility.ParseQueryString(Request.QueryString.ToString()).Get("game");
            if (gameId != null)
            {
                Game = roboGames.Find(game => game.UrlName == gameId);
            }
            else
            {
                // SRB2 2.2 default for testing. Should be changed to throw an error.
                Game = roboGames.Find(game => game.UrlName == "sonicroboblast2v22");
            }

            UserLoggedIn = IsLoggedIn;
        }

        public IActionResult OnPostAsync(ReplayUploadDb fileUpload)
        {
            if (fileUpload == null || fileUpload.FormFiles == null || fileUpload.FormFiles.Count == 0)
            {
                // TODO: Error message thing or something
                return null;
            }

            ErrorMessages.Clear();
            List<IFormFile> files = fileUpload.FormFiles;
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var stream = formFile.OpenReadStream();
                    var bytes = new byte[formFile.Length];
                    stream.Read(bytes, 0, (int)formFile.Length);
                    var rec = new RoboRecord(CurrentUser, bytes.ToArray());

                    // If something went wrong while reading the replay, don't upload it.
                    switch (rec.readStatus)
                    {
                        case RoboRecord.ReadStatus.Success:
                            // The condition SHOULD always be true, but let's be careful
                            if (rec.FileBytes != null && rec.FileBytes.Length > 0)
                            {
                                RecordList.Add(rec);
                            }
                            else
                            {
                                ErrorMessages.Add(new ErrorMessage(formFile.FileName, RoboRecord.MessageErrorUnknown));
                            }
                            break;
                        case RoboRecord.ReadStatus.ErrorInvalid:
                            ErrorMessages.Add(new ErrorMessage(formFile.FileName, RoboRecord.MessageErrorInvalid));
                            break;
                        case RoboRecord.ReadStatus.ErrorReading:
                            ErrorMessages.Add(new ErrorMessage(formFile.FileName, RoboRecord.MessageErrorReading));
                            break;
                        case RoboRecord.ReadStatus.ErrorUnfinished:
                            ErrorMessages.Add(new ErrorMessage(formFile.FileName, RoboRecord.MessageErrorIncomplete));
                            break;
                        case RoboRecord.ReadStatus.ErrorNotSrb2:
                            ErrorMessages.Add(new ErrorMessage(formFile.FileName, RoboRecord.MessageErrorNotSrb2));
                            break;
                        default:
                            ErrorMessages.Add(new ErrorMessage(formFile.FileName, RoboRecord.MessageErrorUnknown));
                            break;
                    }
                }
            }
            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            // return new OkObjectResult(new {count = files.Count, size});
            return null;
        }

        public IActionResult OnPostUploadAsync()
        {
            ErrorMessages.Clear();
            if (!IsLoggedIn)
                return null;

            foreach (var record in RecordList)
            {
                // TODO: Make this less horribly inefficient by only reading the file here
                // Check if this is the best time or worth uploading
                DbSelector.TryGetGameLevelFromMapId(Game.UrlName, record.LevelNumber.ToString(), out RoboLevel level);
                DbInserter.AddRecordIfNeeded(record, level);
            }
            
            return RedirectToPage();
        }
        
        public IActionResult OnPostCancelAsync()
        {
            ErrorMessages.Clear();
            // Refresh the page to cancel the uploads
            return RedirectToPage();
        }

        public IActionResult OnPostDeleteAsync(int rec)
        {
            ErrorMessages.Clear();
            // Should NEVER happen
            if (RecordList.Count <= rec)
            {
                return null;
            }

            RecordList.RemoveAt(rec);
            return null;
        }
    }
}