using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Test.ObjectClasses
{
    public class ConversationClass
    {
        public string Result { get; set; } = "Error";
        public int? LowerUserID { get; set; }
        public int? HigherUserID { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
