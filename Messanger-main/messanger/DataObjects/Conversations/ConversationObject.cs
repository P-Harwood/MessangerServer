using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace messanger.DataObjects.Conversations
{
    public class ConversationObject
    {
        public int conversation_Id { get; set; }
        public int user_id_1 { get; set; }
        public int user_id_2 { get; set; }
        public string? created_at { get; set; }
    }
}
