using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WS.Test.ObjectClasses;

namespace WS.Test.Scripts
{
    public class HashMethodsClass
    {




        // Method for turning raw password into a hash
        public static bool ValidateHash(string password, byte[] salt, byte[] passwordHash)
        {
            // keysize is used for how manye bytes to create for the salt
            const int keySize = 64;
            const int iterations = 350000;

            // Selects what hash algorithim i am going to use
            var hashAlgorithm = HashAlgorithmName.SHA512;

            // Creates the hash
            var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, iterations, hashAlgorithm, keySize);

            // returns boolean true or false of whether passed password matches the password hash and salt
            return passwordHash.SequenceEqual(hash);
        }



        // Method for creating password Hash
        public static HashInformation GenerateHashInformation(string password)
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




    }
}
