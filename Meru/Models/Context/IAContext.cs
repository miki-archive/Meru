using IA.Migrations;
using System.Data.Entity;

namespace IA.Models.Context
{
    [DbConfigurationType(typeof(Configuration))]
    internal class IAContext : DbContext
    {
        public DbSet<Identifier> Identifiers { get; set; }
        public DbSet<CommandState> CommandStates { get; set; }
        public DbSet<ModuleState> ModuleStates { get; set; }

        public IAContext() : base("PostgreSql")
        {
        }
    }
}