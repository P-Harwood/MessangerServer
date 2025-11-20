using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace messanger.DataObjects.WebSocketPackets
{
    public class RegisterObject
    {
        public string PacketHeader = "SendMessage";

        public bool successfull { get; set; }
        public string username { get; set; }
        public string password { get; set; }


        public Dictionary<string, string> returnDictionary()
        {
            if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(username))
            {
                Debug.WriteLine("passed check");
                Dictionary<string, string> dataDict = new Dictionary<string, string>
                {
                    {"username", username },
                    {"password", password}
                };
                return dataDict;
            }
            return null;
        }
    }
}
