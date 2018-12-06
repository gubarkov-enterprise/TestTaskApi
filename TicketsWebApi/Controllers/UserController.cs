using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TicketsWebApi.Models;
using TicketsWebApi.Services;

namespace TicketsWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthenticateService _authenticateService;

        public UserController(IAuthenticateService authenticateService)
        {
            _authenticateService = authenticateService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserAuthData user)
        {
            var existingUser = _authenticateService.Authenticate(user.Email, user.Password);

            if (existingUser == null)
                return BadRequest();

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ticketwebapiservice@secretkey"));

            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, existingUser.Email),
                new Claim(ClaimTypes.Role, existingUser.Role.ToString())
            };

            var tokeOptions = new JwtSecurityToken(
                issuer: "http://localhost:4000",
                audience: "http://localhost:4000",
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return Ok(new
            {
                Token = tokenString,
                role = existingUser.Role.ToString()
            });
        }

        [Route("AddAuditor")]
        [HttpPost, Authorize(Roles = "Administrator")]
        public IActionResult AddAuditor([FromBody] UserAuthData user)
        {
            _authenticateService.Create(new User {Email = user.Email, Role = UserRole.Auditor}, user.Password);
            return Ok();
        }
    }
}