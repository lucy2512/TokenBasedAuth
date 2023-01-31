using Backend.Context;
using Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

            var user = await _applicationDbContext.Users.FirstOrDefaultAsync(x => x.Username == uobj.Username && x.Password == uobj.Password);
            if(user == null)
            {
                return NotFound("User not found!");
            }
            return Ok("Login Successful!");
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] User uobj)
        {
            if(uobj == null)
            {
                return BadRequest();
            }

            await _applicationDbContext.Users.AddAsync(uobj);
            await _applicationDbContext.SaveChangesAsync();
            return Ok("Registration Successful!");

        }
    }
}
