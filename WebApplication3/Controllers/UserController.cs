using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        public UserController(DataContext context)
        {
            _context = context;

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
          if(_context.Users.Any(u => u.email == request.Email))
            {

                return BadRequest("Cannot use the following email.");
            }
          CreatePasswordHash(request.Password,
              out byte[] passwordHash,
              out byte[] passwordSalt);

            var user = new User
            {
                email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerificationToken = CreateRandomToken()

            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User successfully created");
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == request.Email);
            if(user == null)
            {
                return BadRequest("User not found.");
            }

            if (user.Verifiedat == null)
            {
                return BadRequest("Acount not verified");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Password incorrect!");
            }

            return Ok($"Welcome back, {user.email} ");
        }

        [HttpPost("Verify")]
        public async Task<IActionResult> Verify(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
            if (user == null)
            {
                return BadRequest("Token is invalid.");
            }

            user.Verifiedat = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok("User verified");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == email);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            user.PasswordResetToken = CreateRandomToken();
            user.ResetTokenExpires = DateTime.Now.AddDays(1);
            await _context.SaveChangesAsync();
            return Ok("You may reset your password");
        }

        [HttpPost("Reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordReset request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.token);
            if (user == null || user.ResetTokenExpires < DateTime.Now)
            {
                return BadRequest("Invalid token");
            }

            CreatePasswordHash(request.Password,
              out byte[] passwordHash,
              out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;
           
            await _context.SaveChangesAsync();
            return Ok("Password succesffuly reset");
        }


        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            };

        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256(passwordSalt))
            { 
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }

        }


    }
}
