using Backend.Context;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            user.Token = CreateJwt(user);
             return Ok(new
             {
                 Token = user.Token,
                 Message = "Welcome Back!"
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
            if(uobj.Password != uobj.ConfirmPassword)
            {
                return BadRequest(new
                {
                    Message = "Passwords did not match"
                });
            }
            uobj.Password = PasswordHasher.HashPassword(uobj.Password);
            uobj.ConfirmPassword = PasswordHasher.HashPassword(uobj.ConfirmPassword);
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
                return sb.ToString();
            }
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]"))){
                sb.Append("Password should be Alphanumric" + Environment.NewLine);
                return sb.ToString();
            }
           
            if (!(Regex.IsMatch(password, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\],\\[,{,},?,:,;,|,',\\,.,/,~,`,-,=]")))
            {
                sb.Append("Password should contain special character" + Environment.NewLine);
                return sb.ToString();
            }
            return sb.ToString();
        }

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("Apna time aayega");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name,$"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role,user.Role),
                new Claim(ClaimTypes.Email,user.Email)
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
