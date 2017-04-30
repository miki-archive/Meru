using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Models
{
    class IdentifierContext : DbContext
    {
        public DbSet<Identifier> Identifiers { get; set; }
    }

    class Identifier
    {
        [Key]
        [Column("guild_id")]
        public long __GuildId { get; set; }

        [Column("identifier")]
        public string Value { get; set; }

        [NotMapped]
        public ulong GuildId
        {
            get
            {
                unchecked
                {
                    return (ulong)__GuildId;
                }
            }

            set
            {
                unchecked
                {
                    __GuildId = (long)value;
                }
            }
        }
    }
}
