using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PatternPal.LoggingAPI
{
    public class Session
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public Guid ID { get; set; }
        [Required]
        public int ExtensionVersion { get; set; }
        [Required]
        public DateTime StartSession { get; set; }
        public DateTime EndSession { get; set; }
        //how much more or less than gmt
        [Required]
        public int TimeZone { get; set; }
    }
}
