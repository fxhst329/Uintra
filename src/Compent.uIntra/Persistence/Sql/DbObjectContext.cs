﻿using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using uIntra.Comments;
using uIntra.Core;
using uIntra.Core.Activity;
using uIntra.Likes;
using uIntra.Navigation;
using uIntra.Notification;
using uIntra.Subscribe;

namespace Compent.uIntra.Persistence.Sql
{
    public class DbObjectContext : DbContext
    {
        protected Type EntityTypeConfiguration => typeof(EntityTypeConfiguration<>);

        public DbObjectContext() : this("umbracoDbDSN")
        {
        }

        public DbObjectContext(string nameOrConnectionString)
              : base(nameOrConnectionString)
        {
            Configuration.AutoDetectChangesEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<IntranetActivityEntity> IntranetActivityEntities { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<MyLink> MyLinks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Subscribe> Subscribes { get; set; }
        public DbSet<MemberNotifierSetting> MemberNotifierSettings { get; set; }
        public DbSet<MigrationHistory> MigrationHistories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => !string.IsNullOrEmpty(type.Namespace))
            .Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
                type.BaseType.GetGenericTypeDefinition() == EntityTypeConfiguration).ToList();

            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}