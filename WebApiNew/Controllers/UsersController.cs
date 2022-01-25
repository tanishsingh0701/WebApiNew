using AuthenticationPlugin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApiNew.Data;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{

   //second way of attribute routing for all methods so that they call by controller/methodname
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly CinemaDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly AuthService _auth;

        public UsersController(CinemaDbContext dbContext,IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _auth = new AuthService(configuration);
        }


        [HttpPost]
        public IActionResult Register([FromBody] User user)
        {
            var userWithSameEmail = _dbContext.Users.Where(u => u.Email == user.Email).SingleOrDefault();

            if (userWithSameEmail != null)
            {
                return BadRequest("User already exists with same email");
            }
            else
            {
                var userObj = new User
                {
                    Name = user.Name,
                    Email = user.Email,
                    Password = SecurePasswordHasherHelper.Hash(user.Password),
                    Role = "Users"

                };
                _dbContext.Users.Add(userObj);
                _dbContext.SaveChanges();
                return StatusCode(StatusCodes.Status201Created);
            }
        }


            [HttpPost]
            public IActionResult Login([FromBody] User user) 
            {
               var userEmail = _dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
                if (userEmail == null) 
                {
                    return NotFound();
                }

                if (!SecurePasswordHasherHelper.Verify(user.Password, userEmail.Password)) 
                {
                    return Unauthorized();
                }

                var claims = new[]
                     {
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, userEmail.Role)
                     };

                var token = _auth.GenerateAccessToken(claims);

                return new ObjectResult(new
                {
                    access_token = token.AccessToken,
                    expires_in = token.ExpiresIn,
                    token_type = token.TokenType,
                    creation_Time = token.ValidFrom,
                    expiration_Time = token.ValidTo,
                    user_id= userEmail.Id,
                });

            }


        }
    }

