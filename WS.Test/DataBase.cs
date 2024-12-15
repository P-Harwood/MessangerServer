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
        public async Task<JObject> RegisterAccount(string username, byte[] passwordHash, byte[] passwordSalt)
        {
            
            Console.WriteLine(_connectionString);
            try
            {
                bool accountExists = await CheckAccountExisting(username);
                if (!accountExists)
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
                else
                {
                    Console.WriteLine("Account already exists.");
                    return new JObject { ["Result"] = "Account exists" };
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return HTTPBodyExtractor.errorReturn(ex);
            }

        }



        public async Task<Dictionary<string, string>> ReturnAllUsers()
        {
            string query = $"SELECT userid, username FROM users;";
            try
            {
                var userDictionary = new Dictionary<string, string>();

                await using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await using (var command = new NpgsqlCommand(query, connection))
                    {
                        await using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                // Add each user to the dictionary
                                string userId = reader["userid"].ToString();
                                string username = reader["username"].ToString();
                                userDictionary[userId] = username;
                            }
                        }
                    }
                }

                return userDictionary;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }





        // Method for creating password Hash
        public HashInformation generateHash(string password)
        {
            HashInformation returnInformation = new HashInformation
            {
                Result = "Unknown"
            };

            try
            {
                // keysize is used for how manye bytes to create for the salt
                const int keySize = 64;
                const int iterations = 350000;

                // Selects what hash algorithim i am going to use
                var hashAlgorithm = HashAlgorithmName.SHA512;

                // Generates a salt, GetBytes retuns an array of randomised bytes
                byte[] salt = RandomNumberGenerator.GetBytes(keySize);

                // Creates the hash and stores in hash variable
                var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, iterations, hashAlgorithm, keySize);


                // Creates success case object full of hash information
                returnInformation = new HashInformation
                {
                    Result = "OK",
                    PasswordHash = hash,
                    PasswordSalt = salt,
                };

            }
            catch (Exception ex)
            {
                //Log error which occured
                Console.WriteLine("Error occured during creation of hash: ", ex);

                // Error case for hash generation object is created
                returnInformation = new HashInformation
                {
                    Result = "Error",
                    ErrorMessage = ex.Message
                };
            }

            //Creates a json of either the success or fail case of hashInformation and returns it
            return returnInformation;

        }

        // Method for turning raw password into a hash
        public bool checkHash(string password, byte[] salt, byte[] passwordHash)
        {
            // keysize is used for how manye bytes to create for the salt
            const int keySize = 64;
            const int iterations = 350000;

            // Selects what hash algorithim i am going to use
            var hashAlgorithm = HashAlgorithmName.SHA512;


            Console.WriteLine("Password: "+ password);
            var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, iterations, hashAlgorithm, keySize);


            Console.WriteLine($"Password Hash (Hex): {BitConverter.ToString(hash).Replace("-", "")}");
            Console.WriteLine($"Password Hash (Base64): {Convert.ToBase64String(hash)}");

            Console.WriteLine($"paramater Password Hash (Hex): {BitConverter.ToString(passwordHash).Replace("-", "")}");
            Console.WriteLine($"paramater Password Hash (Base64): {Convert.ToBase64String(passwordHash)}");

            Console.WriteLine($"paramater Password salt (Hex): {BitConverter.ToString(salt).Replace("-", "")}");
            Console.WriteLine($"paramater Password salt (Base64): {Convert.ToBase64String(salt)}");
            return passwordHash.SequenceEqual(hash);
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
    


