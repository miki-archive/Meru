using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IA.Models
{
    internal class Identifier
    {
        [Key]
        [Column("GuildId", Order = 0)]
        public long GuildId { get; set; }

        [Key]
        [Column("IdentifierId", Order = 1)]
        public string DefaultValue { get; set; }

        [Column("identifier")]
        public string Value { get; set; }
    }
}