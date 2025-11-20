using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace messanger.DataObjects
{
    public class MessageObject
    {
        public string PacketHeader { get; set; } = "SendMessage";
        public string Sender { get; set; }
        public string Content { get; set; }
        public string Result { get; internal set; }
        public string ErrorMessage { get; internal set; }



    }
}
