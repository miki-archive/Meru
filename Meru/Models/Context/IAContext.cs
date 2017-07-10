using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Models.Context
{
    class IAContext : DbContext
    {
        public DbSet<Identifier> Identifiers { get; set; }
        public DbSet<CommandState> CommandStates { get; set; }
        public DbSet<ModuleState> ModuleStates { get; set; }

        public IAContext() : base()
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
