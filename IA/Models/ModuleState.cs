using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meru.Models
{
    [Table("ModuleStates")]
    class ModuleState
    {
        [Key]
        [Column("ModuleName", Order = 0)]
        public string ModuleName { get; set; }

        [Key]
        [Column("ChannelId", Order = 1)]
        public long ChannelId { get; set; }

        [Column("State")]
        public bool State { get; set; }
    }
}
