
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace messanger.DataObjects.LocalDataPackets
{
    public class ServerListObj
    {
        public void ProcessJsonResponse(string jsonServerMessage)
        {
            try
            {
                // Clear Users so it is fresh
                Users = new ObservableCollection<UserData>();

                // Read the Json
                using (JsonDocument document = JsonDocument.Parse(jsonServerMessage))
                {
                    JsonElement root = document.RootElement;

                    // Get message out of json TODO: Check if needed probably not
                    string message = root.GetProperty("message").GetString();

                    // Get JsonElement users
                    JsonElement users = root.GetProperty("users");

                    // For loop going through users
                    foreach (JsonElement userElement in users.EnumerateArray())
                    {
                        // Get the Id out of json
                        int id = userElement.GetProperty("Id").GetInt32();

                        // Get the string username out of the Json Element
                        string username = userElement.GetProperty("Username").GetString();

                        // Create new UserData and add it to the UserData List
                        Users.Add(new UserData { Id = id, Username = username });

                    }

                    Result = "Success";
                }
            }catch(JsonException ex)
            {
                Result = "Error";
                ErrorMessage = $"JSON Parsing Error: {ex.Message}";
            }catch(Exception ex)
            {
                Result = "Error";
                ErrorMessage = $"Unexpected Error: {ex.Message}";
            }
            
        }

        public string Result { get; set; } = "Error";
        public ObservableCollection<UserData> Users { get; set; } = new ObservableCollection<UserData>();
        public string ErrorMessage { get; set; }
    }

    
}
