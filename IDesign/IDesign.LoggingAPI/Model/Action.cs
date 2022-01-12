using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace IDesign.LoggingAPI
{
    public class Action
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string? CommitID { get; set; }
        public int ExerciseID { get; set; }
        [Required]
        public DateTime Time { get; set; }
        [Required, ForeignKey("SessionID")]
        public virtual Guid SessionID { get; set; }
        [Required, ForeignKey("ActionTypeID")]
        public virtual string ActionTypeID { get; set; }
        [Required, ForeignKey("ModeID")]
        public virtual string ModeID { get; set; }
    }
}
