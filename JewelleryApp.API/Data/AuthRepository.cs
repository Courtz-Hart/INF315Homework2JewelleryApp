using System;
using System.Threading.Tasks;
using JewelleryApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace JewelleryApp.API.Data
{

    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);  //Finds to see if you have an exisiting user matching the given username

            if (user == null)       //If the user is null, and does not match
            {
                return null;
            }


            //because we get user from repository, we have access to the password salt and password hash
            if(!VerifyPasswordHash(password, user.PasswordSalt, user.PasswordHash)) //This method whether the passwords match or not, and returns true or false.
            {
                return null;
            }

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordSalt, byte[] passwordHash)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))   //You must pass it the key (passwordSalt) so you can compute the hash.
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); 
                for (int i =0; i<computedHash.Length; i++)
                {
                    if(computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;      //These values are going to be stored in the byte array variables
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }


        //Setting passwordHash and passwordSalt by the randomly generated key through HMACSHA512
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())     //Call to create a new instance of class
            {
                //This will all be immediately disposed of as soon as the method has finished, through the HMACSHA512 method including the method Dispose
                passwordSalt = hmac.Key;       //Set password salt, HMACSHA512 provides us with a randomly generated key
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); //ComputerHash takes a byte array, so you have to encode password to a byte array to pass it.
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync( x => x.Username == username))
            {
                return true;
            }

            return false;
        }
    }
}