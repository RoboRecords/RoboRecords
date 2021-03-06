/*
 * RoboCharacter.cs
 * Copyright (C) 2022, Ors <Riku-S> and Refrag <Refragg>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * See the 'LICENSE' file for more details.
 */

using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace RoboRecords.Models
{
    public class RoboCharacter
    {
        [JsonPropertyName("id")]
        public int DbId { get; set; }
        
        public string Name { get; set; }
        public string NameId;
        public string IconUrl;

        public RoboCharacter(string name, string nameId = null)
        {
            Name = name;
            
            var regex = new Regex("[^a-zA-Z0-9_-]");
            nameId ??= regex.Replace(name, "").ToLower();
            NameId = nameId;

            IconUrl = FileManager.AssetsDirectoryName + "/images/characters/" + nameId + ".png";
        }

        // Needed for the database context
        public RoboCharacter()
        {
            
        }
        
        public static bool operator ==(RoboCharacter character1, RoboCharacter character2)
        {
            if (character1 is null && character2 is not null || character1 is not null && character2 is null)
                return false;
            
            return character1 is null || character1.NameId == character2.NameId;
        }

        public static bool operator !=(RoboCharacter character1, RoboCharacter character2) => !(character1 == character2);
    }
}