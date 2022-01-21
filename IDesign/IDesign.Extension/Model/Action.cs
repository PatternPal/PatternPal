using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDesign.Extension.Model
{
    public class Action
    {
        public Action(int id, string commitId, int exerciseNumber, DateTime time, string sessionId, string actionTypeName, string modeName)
        {
            ID = id; 
            CommitID = commitId;
            ExerciseNumber = exerciseNumber;
            Time = time;
            SessionID = sessionId;
            ActionTypeID = actionTypeName;
            ModeID = modeName;
        }

        public Action(string actionTypeName)
        {
            ActionTypeID = actionTypeName;
            CommitID = "commitid";
            ExerciseNumber = 2;
            Time = DateTime.Now;
            SessionID = "79f83fbd-a9ed-434e-b585-e9258f804012";
            ModeID = IDesignExtensionPackage.CurrentMode.ToString();
        }

        public int ID { get; set; }
        public string CommitID { get; set; }
        public int ExerciseNumber { get; set; }
        public DateTime Time { get; set; }
        public virtual string SessionID { get; set; }
        public virtual string ActionTypeID { get; set; }
        public virtual string ModeID { get; set; }
    }
}
