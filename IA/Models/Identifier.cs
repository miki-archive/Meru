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
    class Identifier
    {
        [Key]
        [Column("GuildId")]
        public long GuildId { get; set; }

        [Key]
        [Column("IdentifierId")]
        public string DefaultValue { get; set; }

        [Column("identifier")]
        public string Value { get; set; }
    }
}
