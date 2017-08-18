namespace IA.Migrations
{
    using EFCache;
    using EFCache.RedisCache;
    using StackExchange.Redis;
    using System;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Core.Common;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Migrations;

    internal sealed class MigrationConfiguration : DbMigrationsConfiguration<IA.Models.Context.IAContext>
    {
        public MigrationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
        }
    }

    public class Configuration : DbConfiguration
    {
        public Configuration() : base()
        {
        }
    }
}