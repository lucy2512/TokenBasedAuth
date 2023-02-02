using Backend.Context;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.RegularExpressions;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public UserController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] User uobj)
        {
            if(uobj == null)
            {
                return BadRequest();
            }

            var user = await _applicationDbContext.Users.FirstOrDefaultAsync(x => x.Username == uobj.Username);
            if(user == null)
            {
                 return Unauthorized(new
                 {
                     Message = "User not found!"
                 });
                //return StatusCode(StatusCodes.Status401Unauthorized, "User not found");
            }
            if(!(PasswordHasher.VerifyPassword(uobj.Password, user.Password)))
            {
                return BadRequest(new
                {
                    Message = "Password is incorrect"
                });
            }
             return Ok(new
             {
                 Message = "Login Successful!"
             });
            //return StatusCode(StatusCodes.Status200OK, "Login Success");
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] User uobj)
        {
            if(uobj == null)
            {
                return BadRequest();
            }

            if(await CheckUserNameExist(uobj.Username))
            {
                return BadRequest(new
                {
                    Message = "Username already exist!"
                });
            }

            if(await CheckEmailExist(uobj.Email))
            {
                return BadRequest(new
                {
                    Message = "Email is already in use"
                });
            }
            var pw = CheckPasswordStrength(uobj.Password);
            if (!string.IsNullOrEmpty(pw))
            {
                return BadRequest(new
                {
                    Message = pw.ToString()
                });
            }


            uobj.Password = PasswordHasher.HashPassword(uobj.Password);
            uobj.Token = "";
            uobj.Role = "User";
            await _applicationDbContext.Users.AddAsync(uobj);
            await _applicationDbContext.SaveChangesAsync();
            return Ok(new
            {
                Message = "Registration Success"
            });
           //return StatusCode(StatusCodes.Status200OK,"Registration Success");

        }

        private Task<bool> CheckUserNameExist(string username)
           => _applicationDbContext.Users.AnyAsync(x => x.Username == username);


        private Task<bool> CheckEmailExist(string email)
           => _applicationDbContext.Users.AnyAsync(x => x.Email == email);


        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if (password.Length < 4)
            {
                sb.Append("Minimum password length should be 8" + Environment.NewLine);
            }
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]"))){
                sb.Append("Password should be Alphanumric" + Environment.NewLine);
            }
           
            if (!(Regex.IsMatch(password, "[<,>]")))
            {
                sb.Append("Password should contain special character" + Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
