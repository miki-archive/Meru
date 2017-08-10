using System.Data.Entity;

namespace IA.Models.Context
{
    internal class IAContext : DbContext
    {
        public DbSet<Identifier> Identifiers { get; set; }
        public DbSet<CommandState> CommandStates { get; set; }
        public DbSet<ModuleState> ModuleStates { get; set; }

        public IAContext() : base("PostgreSql")
        {
        }

        internal static IAContext CreateNoCache()
        {
            IAContext m = new IAContext();
            m.Configuration.LazyLoadingEnabled = false;
            return m;
        }
    }
}