using messanger.WrapperClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace messanger.DataObjects.WebSocketPackets
{
    public class SignInObject
    {
        public string PacketHeader { get; set; } = "SignIn";
        public required string username { get; set; }
        public string password { get; set; } // TODO MAYBE ADD PASSWORD


        // TODO add validation to assigning 


        // Compiles data into dictionary to be sent off
        public Result<Dictionary<string, string>> returnDictionary()
        {
            
            if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(username))
            {
                Dictionary<string, string> dataDict = new Dictionary<string, string>
                {
                    {"username", username},
                    {"password", password}
                };
                return Result<Dictionary<string,string>>.Success(dataDict);
            }
            return Result<Dictionary<string, string>>.Failure("Username Or Password is null");
        }
    }
}
