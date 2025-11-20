using messanger.WrapperClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace messanger.DataObjects.Conversations
{
    public class NewConversRequest
    {
        public string? LowerUserID { get; set; }
        public string? HigherUserID { get; set; }


        public Result<Dictionary<string, string>> ReturnDictionary()
        {
            Debug.WriteLine("Creating ConversationObject with IDS:", LowerUserID, HigherUserID);

            //If the user ids are not null then return them with success
            if (LowerUserID != null && HigherUserID != null)
            {
                var dataDict = new Dictionary<string, string>
                {
                    { "user_ID_1", LowerUserID },
                    { "user_ID_2", HigherUserID }
                };
                // Return wrapped success result
                return Result<Dictionary<string, string>>.Success(dataDict);
            }

            // return wrapped fail
            return Result<Dictionary<string, string>>.Failure("LowerUserID or HigherUserID is null.");
        }
    }


}
