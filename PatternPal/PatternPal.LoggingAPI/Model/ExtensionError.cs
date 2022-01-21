using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace PatternPal.LoggingAPI
{
    public class ExtensionError
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int ID { get; set; }
        public string Message { get; set; }
        [Required, ForeignKey("ActionID")]
        public virtual int ActionID { get; set; }
    }
}
