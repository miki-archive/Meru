namespace IA.Migrations
{
    using EFCache;
    using EFCache.Redis;
    using System;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Core.Common;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<IA.Models.Context.IAContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(IA.Models.Context.IAContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
    public class CacheConfiguration : DbConfiguration
    {
        public CacheConfiguration()
        {
            var redisConnection = ConfigurationManager.ConnectionStrings["Redis"].ToString();
            var cache = new RedisCache(redisConnection);
            var transactionHandler = new CacheTransactionHandler(cache);
            AddInterceptor(transactionHandler);

            Loaded += (sender, args) =>
            {
                args.ReplaceService<CachingProviderServices>(
                    (s, _) => new CachingProviderServices(s, transactionHandler, new RedisCachingPolicy())
                    );
            };

        }
    }

    public class RedisCachingPolicy : CachingPolicy
    {
        protected override void GetExpirationTimeout(ReadOnlyCollection<EntitySetBase> affectedEntitySets, out TimeSpan slidingExpiration, out DateTimeOffset absoluteExpiration)
        {
            slidingExpiration = TimeSpan.FromMinutes(5);
            absoluteExpiration = DateTimeOffset.Now.AddMinutes(30);
        }
    }
}