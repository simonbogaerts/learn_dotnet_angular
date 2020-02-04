using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Bogsi.DatingApp.API.Models;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Bogsi.DatingApp.API.Data.Seed
{
    public class Seed
    {
        public static void SeedUsers(DataContext context)
        {
            if (!context.Users.Any())
            {
                var users = JsonConvert.DeserializeObject<List<User>>(
                    System.IO.File.ReadAllText("Data/Seed/UserSeedData.json"));

                foreach (var user in users)
                {
                    CreatePasswordHash("password", out var passwordHash, out var passwordSalt);
                    user.Hash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.Username = user.Username.ToLower();

                    context.Users.Add(user);
                }

                context.SaveChanges();
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();

            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}