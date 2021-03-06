/*
 * Login.cshtml.cs: Backend for the login website's page
 * Copyright (C) 2022, Refrag <Refragg> and Zenya <Zeritar>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * See the 'LICENSE' file for more details.
 */

using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoboRecords.DbInteraction;
using RoboRecords.Models;
using RoboRecords.Services;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace RoboRecords.Pages
{
    public class Login : RoboPageModel
    {
        private RoboUserManager _roboUserManager;
        private SignInManager<IdentityRoboUser> _signInManager;
        
        public string Token;

        public Login(RoboUserManager roboUserManager, SignInManager<IdentityRoboUser> signInManager, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _roboUserManager = roboUserManager;
            _signInManager = signInManager;
        }

        public void OnGet()
        {
            // Improper example of checking if current user is a moderator. To test, manually change Roles column of your IdentityRoboUser entry to 3 and log in.
            if (IsLoggedIn)
            {
                isModerator = Validator.UserHasRequiredRoles(CurrentIdentityUser, UserRoles.Moderator);
            }
        }

        public class RegisterData
        {
            public string Email { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string PasswordConfirmation { get; set; }
        }

        public IActionResult OnPostRegister([FromBody] RegisterData data)
        {
            string email = data.Email;
            string usernamewithdiscrim = data.Username;
            string password = data.Password;
            string confirmedPassword = data.PasswordConfirmation;

            if (password != confirmedPassword)
            {
                return BadRequest("Password Confirmation Error");
            }

            string[] splittedUsername = Validator.TrySplitUsername(usernamewithdiscrim);

            string username = splittedUsername[0];
            short discriminator = short.Parse(splittedUsername[1]);
            
            if (discriminator == 0)
            {
                return BadRequest("No Discriminator");
            }

            Logger.Log(username, Logger.LogLevel.Debug, true);
            
            // TODO: Move this to RoboUserManager
            // Try to create new IdentityUser and if it succeeds, create a RoboUser with the same username.
            IdentityResult userCreationResult = _roboUserManager.Create(email, username, discriminator, password);
            if (userCreationResult.Succeeded)
            {
                DbInserter.AddRoboUser(new RoboUser(username, discriminator));
                return Content("Success");
            }

            return BadRequest(userCreationResult.Errors);
        }
        
        public class LoginData
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
        
        public IActionResult OnPostLogin([FromBody] LoginData data)
        {
            string usernamewithdiscrim = data.Email;
            string password = data.Password;
            
            string[] splittedUsername = Validator.TrySplitUsername(usernamewithdiscrim);

            string username = splittedUsername[0];
            short discriminator = short.Parse(splittedUsername[1]);

            IdentityRoboUser userToLogin;

            if (discriminator == 0)
            {
                // Invalid username format. Check if they entered an email address instead.
                if (Validator.ValidateEmail(usernamewithdiscrim))
                {
                    DbSelector.TryGetIdentityUserFromEmail(usernamewithdiscrim, out userToLogin);
                    if (userToLogin is null)
                        return BadRequest("No user with this email address was found");
                }
                else
                    return BadRequest("Invalid username/email entered.");
            }
            else
            {
                // IdentityUser has discrim included in username
                DbSelector.TryGetIdentityUserFromUserName(usernamewithdiscrim, out userToLogin);
                if (userToLogin is null)
                    return BadRequest("No user with this username / discriminator combination was found");
            }

            SignInResult result = _signInManager.PasswordSignInAsync(userToLogin, password, true, false).Result;
            
            if (result.Succeeded)
            {
                Logger.Log("Success", true);
                isModerator = Validator.UserHasRequiredRoles(userToLogin, UserRoles.Moderator);
                return Content("Success");
            }
            
            return BadRequest("The password for this user is invalid");
        }
    }
}