using canonapi.Authentication;
using canonapi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace canonapi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthenticateController : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthenticateController(ApplicationDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _dbContext = context;
        }

        [HttpPost]
        [ActionName("Login")]
        public IActionResult Login([FromBody] User model)
        {
            User userObj = _dbContext.Users.SingleOrDefault(user => user.username == model.username
            && user.userpassword == model.userpassword);

            if (userObj != null)
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userObj.firstname),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
    }
}
