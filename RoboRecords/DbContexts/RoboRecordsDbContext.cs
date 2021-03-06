/*
 * RoboRecordsDbContext.cs: The model definitions for the entities stored in the MySql database
 * Copyright (C) 2022, Refrag <Refragg>, Zenya <Zeritar> and Ors <Riku-S>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * See the 'LICENSE' file for more details.
 */

using System.Text;
using Microsoft.EntityFrameworkCore;
using RoboRecords.Models;

namespace RoboRecords.DatabaseContexts
{
    public partial class RoboRecordsDbContext : DbContext
    {
        private static string _connectionString;

        public static void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public DbSet<LevelGroup> LevelGroups { get; set; }
        public DbSet<RoboGame> RoboGames { get; set; }

        public DbSet<RoboUser> RoboUsers { get; set; }
        public DbSet<RoboCharacter> RoboCharacters { get; set; }

        public DbSet<RoboLevel> RoboLevels { get; set; }

        public DbSet<RoboRecord> RoboRecords { get; set; }
        
        public DbSet<SiteAsset> SiteAssets { get; set; }
        public DbSet<CharacterAsset> CharacterAssets { get; set; }
        public DbSet<GameAsset> GameAssets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RoboGame>(entity =>
            {
                entity.HasKey(e => e.DbId);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.UrlName).IsRequired();
                entity.Property(e => e.IconPath).IsRequired();
                entity.HasMany(e => e.LevelGroups).WithOne(e => e.RoboGame);
            });

            modelBuilder.Entity<LevelGroup>(entity =>
            {
                entity.HasKey(e => e.DbId);
                entity.Property(e => e.Name).IsRequired();
                entity.HasMany(e => e.Levels).WithOne(e => e.LevelGroup);
                entity.Property(e => e.WriteLevelNames).IsRequired();
            });
            
            modelBuilder.Entity<RoboUser>(entity =>
            {
                entity.HasKey(e => e.DbId);
                entity.Property(e => e.Discriminator).IsRequired();
                entity.Property(e => e.UserNameNoDiscrim).IsRequired();
            });
            
            modelBuilder.Entity<RoboLevel>(entity =>
            {
                entity.HasKey(e => e.DbId);
                entity.Property(e => e.IconUrl).IsRequired();
                entity.Property(e => e.LevelNumber).IsRequired().UsePropertyAccessMode(PropertyAccessMode.Property);
                entity.HasMany(e => e.Records).WithOne(e => e.Level);
                entity.Property(e => e.LevelName).IsRequired();
                entity.Property(e => e.Act).IsRequired();
                entity.Property(e => e.Nights).IsRequired();
            });

            modelBuilder.Entity<RoboCharacter>(entity =>
            {
                entity.HasKey(e => e.DbId);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.NameId).IsRequired();
                entity.Property(e => e.IconUrl).IsRequired();
            });

            modelBuilder.Entity<RoboRecord>(entity =>
            {
                entity.HasKey(e => e.DbId);
                entity.HasOne(e => e.Uploader);
                entity.Property(e => e.Tics).IsRequired();
                entity.Property(e => e.Rings).IsRequired();
                entity.Property(e => e.Score).IsRequired();
                entity.HasOne(e => e.Character);

                entity.Property(e => e.UploadTime).IsRequired();

                entity.Property(e => e.LevelNumber).IsRequired();

                entity.Property(m => m.Description).HasMaxLength(256);
            });
            
            modelBuilder.Entity<SiteAsset>(entity =>
            {
                entity.HasKey(e => e.DbId);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.FileExtension).IsRequired();
            });
            
            modelBuilder.Entity<GameAsset>(entity =>
            {
                entity.HasKey(e => e.DbId);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.FileExtension).IsRequired();
                entity.HasOne(e => e.Game);
            });
            
            modelBuilder.Entity<CharacterAsset>(entity =>
            {
                entity.HasKey(e => e.DbId);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.FileExtension).IsRequired();
                entity.HasOne(e => e.Character);
            });
        }
    }
}