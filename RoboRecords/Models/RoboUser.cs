/*
 * RoboUser.cs: the user model as stored in the database
 * Copyright (C) 2022, Refrag <Refragg> and Zenya <Zeritar>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * See the 'LICENSE' file for more details.
 */

using System;
using System.IO;
using Microsoft.AspNetCore.Identity;

namespace RoboRecords.Models
{
    public class RoboUser
    {
        public int DbId;

        public short Discriminator { get; set; }

        public string UserNameNoDiscrim { get; set; }

        public string AvatarPath => GetAvatarPath();

        public string UserName => $"{UserNameNoDiscrim}#{Discriminator.ToString().PadLeft(4,'0')}";

        public RoboUser(string userName, short numberDiscriminator)
        {
            Discriminator = numberDiscriminator;
            UserNameNoDiscrim = userName;
        }

        // Needed for the database context
        public RoboUser()
        {
            
        }

        private string GetAvatarPath()
        {
            string avatarPath = $"{FileManager.UserAssetsDirectoryName}/{DbId}/avatar.png";

            if (!FileManager.Exists(avatarPath))
                return $"{FileManager.AssetsDirectoryName}/guest.png";

            return avatarPath;
        }
        
        public static bool operator ==(RoboUser user1, RoboUser user2)
        {
            if (user1 is null && user2 is not null || user1 is not null && user2 is null)
                return false;
            
            return user1 is null || (user1.Discriminator == user2.Discriminator && user1.UserNameNoDiscrim == user2.UserNameNoDiscrim);
        }

        public static bool operator !=(RoboUser user1, RoboUser user2) => !(user1 == user2);
    }
}