using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Test.ObjectClasses
{
    internal class UserListObject
    {
        public readonly string PacketHeader = "UserList";

        public JArray userList { get; set; }

    }
}
    
