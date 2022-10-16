using System.Security.Cryptography;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController  : BaseApiController
    {
        public readonly DataContext _context;
        public AccountController (DataContext context)
        {
            _context = context;    
        }

        [HttpPost("Register")]  //POST api/account/register
        public async Task<ActionResult<AppUser>> Register(LoginDto registerDto)
        {
            if (await UserExists(registerDto.UserName)) return BadRequest("Username is taken"); 

            using var hmac = new HMACSHA512();

            var user = new AppUser {
                UserName = registerDto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();
            
            return user;
        }

        [HttpPost("Login")]  //POST api/account/login
        public async Task<ActionResult<AppUser>> Login(RegisterDto loginDto)
        {
            // get user from db
            var user = await this._context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());
            if (user == null) return Unauthorized("Invalid username or password");

            // check password ... so using hmac to do the reverse of what we did to register
            // calculate the hash using the salt and the given password

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computeHash.Length; i++)
            {
                if (computeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid username or password");
            }

            return user;
        
        }


    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }

    }
}