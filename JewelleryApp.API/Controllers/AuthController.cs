using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JewelleryApp.API.Data;
using JewelleryApp.API.Dtos;
using JewelleryApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JewelleryApp.API.Controllers
{

    [Route("api/[controller]")]             //Attribute based routing
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userForRegisterDto.Username))
            {
                return BadRequest("Username already exists");
            }

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password); //Check to see we have a user and their username and password matches
            
            if (userFromRepo == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.ID.ToString()), //First CLaim will be the userID
                new Claim(ClaimTypes.Name, userFromRepo.Username)       //Second Claim will be the username
            };

            //System then needs to check that the token being sent back is a valid user token            

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));       //Creating Security Key

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);        //Use key as sign in credentials, and encrypting the key with a hash algorithm

            //Creating Token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),       //claim is passed as subject
                Expires = DateTime.Now.AddDays(1),          //Can change token days
                SigningCredentials = creds
            };

            var TokenHandler = new JwtSecurityTokenHandler();       

            var token = TokenHandler.CreateToken(tokenDescriptor);          //Create Token using the tokenDescriptor above

            return Ok(new {
                token = TokenHandler.WriteToken(token)      //Write token into response that we send back to the client
            });
        }
    }
}