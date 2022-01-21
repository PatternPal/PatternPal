using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PatternPal.LoggingAPI
{
    public class Mode
    {
        /// <summary>
        /// This is the name AND id of the Mode
        /// </summary>
        [Required]
        [Key]
        public string ID { get; set; }
    }
}
