using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;
using WS.Test.ObjectClasses;
using Newtonsoft.Json.Linq;
using WS.Test.Scripts;

namespace WS.Test
{
    internal class DataBase
    {
        private readonly string _connectionString;

        // Constructor to initialize connection string
        public DataBase()
        {
            string DBhostIP = "localhost";
            string DBport = "5432";
            string DBusername = "postgres";
            string DBpassword = "password";
            string DBdatabase = "postgres";

            _connectionString = $"Host={DBhostIP};Port={DBport};Username={DBusername};Password={DBpassword};Database={DBdatabase};";
        }

        // Method to execute non-query commands like INSERT, UPDATE, DELETE
        public void RunQuery(string query)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }




        // Method to execute a query and get a single string result
        public async Task<string> GetQueryResult(string query)
        {
            try
            {
                await using (var connection = new NpgsqlConnection(_connectionString))
                {
                    
                    await connection.OpenAsync();
                    await using (var command = new NpgsqlCommand(query, connection))
                    {
                        await using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Example: return the first column as a string
                                return reader.GetString(0);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return null;
        }

        // Method to check login details (to be implemented)
        public void CheckLoginDetails(string username, string password)
        {
            
        }

        // Method to register a new account
        public async Task<JObject> CreateNewAccount(string username, byte[] passwordHash, byte[] passwordSalt)
        {
            
            Console.WriteLine(_connectionString);
            try
            {
                    // Use parameterized query to safely handle inputs
                    string query = "INSERT INTO users (username, userPasswordHash, userPasswordSalt) VALUES (@username, @passwordHash, @passwordSalt);";

                    // Use using block to ensure the connection is properly disposed
                    using (var connection = new NpgsqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new NpgsqlCommand(query, connection))
                        {
                            // Add parameters to the command to prevent SQL injection and handle byte arrays correctly
                            command.Parameters.AddWithValue("@username", NpgsqlTypes.NpgsqlDbType.Varchar, username);
                            command.Parameters.AddWithValue("@passwordHash", NpgsqlTypes.NpgsqlDbType.Bytea, passwordHash);
                            command.Parameters.AddWithValue("@passwordSalt", NpgsqlTypes.NpgsqlDbType.Bytea, passwordSalt);

                            await command.ExecuteNonQueryAsync(); // Execute the query asynchronously
                        }
                    }

                    Console.WriteLine("Account registered successfully.");
                    return new JObject { ["Result"] = "OK" };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return RequestBodyFormatter.errorReturn(ex);
            }

        }






        // Passed output of
        public async Task<bool> CheckConversationExists(ConversationClass conversation)
        {
            string query = $"SELECT COUNT(*) FROM conversations WHERE user_ID_1 = @LowerUserID AND user_ID_2 = @HigherUserID;";
            try
            {
                await using (var connection = new NpgsqlConnection(_connectionString))
                {

                    await connection.OpenAsync();
                    await using (var command = new NpgsqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@LowerUserID", conversation.LowerUserID);
                        command.Parameters.AddWithValue("@HigherUserID", conversation.HigherUserID);
                        var count = (long)await command.ExecuteScalarAsync();

                        return count > 0;
                    }
                }
            }
            catch (NpgsqlException npgsqlEx)
            {
                Console.WriteLine($"Database error: {npgsqlEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return false;
        }

        public async Task<JObject> AddNewConversation(ConversationClass conversation)
        {



            try
            {
                // Use parameterized query to safely handle inputs
                string query = "INSERT INTO conversations (user_ID_1, user_ID_2) VALUES (@user_ID_1, @user_ID_2);";

                // Use using block to ensure the connection is properly disposed
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        // Add parameters to the command to prevent SQL injection and handle byte arrays correctly
                        command.Parameters.AddWithValue("@user_ID_1", NpgsqlTypes.NpgsqlDbType.Integer, conversation.LowerUserID);
                        command.Parameters.AddWithValue("@user_ID_2", NpgsqlTypes.NpgsqlDbType.Integer, conversation.HigherUserID);

                        await command.ExecuteNonQueryAsync(); // Execute the query asynchronously
                    }
                }

                Console.WriteLine("Account registered successfully.");
                return new JObject { ["Result"] = "OK" };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return RequestBodyFormatter.errorReturn(ex);
            }

        }





        // Gets all users from database, test fuction 
        public async Task<List<UserData>> ReturnAllUsers()
        {
            // Ensure only user id and username are grabbed to prevent vulnerability
            string query = "SELECT userid, username FROM users;";

            try
            {
                var users = new List<UserData>();

                await using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await using (var command = new NpgsqlCommand(query, connection))
                    {
                        await using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                // Add each user to the list
                                var user = new UserData
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("userid")),
                                    Username = reader.GetString(reader.GetOrdinal("username"))
                                };

                                users.Add(user);
                            }
                        }
                    }
                }

                return users;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }















        // Method to check if an account exists
        public async Task<bool> CheckAccountExisting(string username)
        {
            string query = $"SELECT COUNT(*) FROM users WHERE username = @username;";
            try
            {
                await using (var connection = new NpgsqlConnection(_connectionString))
                {

                    await connection.OpenAsync();
                    await using (var command = new NpgsqlCommand(query, connection))
                    {
  
                        command.Parameters.AddWithValue("@username", username);
                        var count = (long) await command.ExecuteScalarAsync();

                        return count > 0;
                    }
                }
            }
            catch (NpgsqlException npgsqlEx)
            {
                Console.WriteLine($"Database error: {npgsqlEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return false;
        }

        // Method to check if an account exists
        public async Task<AccInfoObj> GetAccountDetailsAsync(string username, byte[] passwordHash)
        {
            string query = "SELECT userID, username, userPasswordHash, userPasswordSalt FROM users WHERE username = @username AND userPasswordHash = @passwordHash LIMIT 1;";

            try
            {
                await using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@passwordHash", passwordHash);

                        await using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new AccInfoObj
                                {
                                    Result = "OK",
                                    userId = reader.GetInt32(reader.GetOrdinal("userID")),
                                    username = reader.GetString(reader.GetOrdinal("username")),
                                };
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException npgsqlEx)
            {
                Console.WriteLine($"Database error: {npgsqlEx.Message}");
                return new AccInfoObj
                {
                    Result = "Error",
                    errorMessage = npgsqlEx.Message
                };
            }
        
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new AccInfoObj
                {
                    Result = "Error",
                    errorMessage = ex.Message
                };
            }

            return new AccInfoObj
            {
                Result = "NONE",
                errorMessage = "NO USER"
            };
        }

        public async Task<AccInfoObj> GetAccountByName(string username)
        {
            string query = "SELECT userID, username, userPasswordHash, userPasswordSalt FROM users WHERE username = @username LIMIT 1;";

            try
            {
                await using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);

                        await using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                long saltLength = reader.GetBytes(reader.GetOrdinal("userpasswordsalt"), 0, null, 0, 0);
                                // Create a buffer to hold the data
                                byte[] passwordSalt = new byte[saltLength];
                                reader.GetBytes(reader.GetOrdinal("userpasswordsalt"), 0, passwordSalt, 0, (int)saltLength);

                                long hashLength = reader.GetBytes(reader.GetOrdinal("userpasswordhash"),0,null, 0, 0);
                                byte[] passwordHash = new byte[hashLength];
                                reader.GetBytes(reader.GetOrdinal("userpasswordhash"), 0, passwordHash, 0, (int)hashLength);

                                return new AccInfoObj
                                {
                                    Result = "OK",
                                    userId = reader.GetInt32(reader.GetOrdinal("userID")),
                                    username = reader.GetString(reader.GetOrdinal("username")),
                                    passwordSalt = passwordSalt,
                                    passwordHash = passwordHash
                                };
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException npgsqlEx)
            {
                Console.WriteLine($"Database error: {npgsqlEx.Message}");
                return new AccInfoObj
                {
                    Result = "Error",
                    errorMessage = npgsqlEx.Message
                };
            }

            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new AccInfoObj
                {
                    Result = "Error",
                    errorMessage = ex.Message
                };
            }

            return new AccInfoObj
            {
                Result = "NONE",
                errorMessage = "NO USER"
            };
        }
    }


}
    


