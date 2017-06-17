using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meru.Models
{
    class Identifier
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
