﻿using FreeCourse.IdentityServer.Dtos;
using FreeCourse.IdentityServer.Models;
using FreeCourse.Shared.Dtos;
using IdentityServer4.Hosting.LocalApiAuthentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace FreeCourse.IdentityServer.Controllers
{
    [Authorize(LocalApi.PolicyName)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(UserManager<ApplicationUser> userManager)
        {
                _userManager    = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(SignupDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                City= dto.City, 
            };
         
            var result=await  _userManager.CreateAsync(user,dto.Password);

            if (!result.Succeeded) 
            {
                return BadRequest(Response<NoContent>.Fail(result.Errors.Select(x=>x.Description).ToList(),400));
            }

            return NoContent();

        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var userIdClaim=User.Claims.FirstOrDefault(x=>x.Type==JwtRegisteredClaimNames.Sub);// sub:token sahibinin kimliğini belirtir

            if (userIdClaim == null) { return BadRequest(); }

            var user = await _userManager.FindByIdAsync(userIdClaim.Value);
            if (user == null) { return BadRequest(); }


            return Ok(new { Id = user.Id, UserName = user.UserName, Email = user.Email, City = user.City });

        }
    }
}
