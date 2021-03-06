/*
 * RoboPageModel.cs: The page model used accros all pages
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoboRecords.DbInteraction;

namespace RoboRecords.Models
{
    public class RoboPageModel : PageModel
    {
        private IHttpContextAccessor _httpContextAccessor;

        private bool _initialized = false;
        
        public RoboPageModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            _currentUser = new Lazy<RoboUser>(GetUserAndUpdateResult);
            _currentIdentityUser = new Lazy<IdentityRoboUser>(GetIdentityUserAndUpdateResult);
        }
        
        public bool IsLoggedIn
        {
            get
            {
                if (_currentIdentityUser.IsValueCreated)
                    return _foundUserLastResult;
                else
                {
                    GetUserAndUpdateResult();
                    return _foundUserLastResult;
                }
            }
        }

        public bool isModerator = false;

        private Lazy<RoboUser> _currentUser;
        
        public RoboUser CurrentUser => _currentUser.Value;

        private bool _foundUserLastResult;
        

        private Lazy<IdentityRoboUser> _currentIdentityUser;
        
        public IdentityRoboUser CurrentIdentityUser => _currentIdentityUser.Value;

        private bool _foundIdentityUserLastResult;

        private RoboUser GetUserAndUpdateResult()
        {
            RoboUser roboUser;
            _foundUserLastResult = DbSelector.TryGetRoboUserFromUserName(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name), out roboUser);
            return roboUser;
        }
        
        private IdentityRoboUser GetIdentityUserAndUpdateResult()
        {
            IdentityRoboUser identityRoboUser;
            _foundIdentityUserLastResult = DbSelector.TryGetIdentityUserFromUserName(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name), out identityRoboUser);
            return identityRoboUser;
        }
    }
}