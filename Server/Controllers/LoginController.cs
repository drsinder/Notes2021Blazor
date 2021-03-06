﻿/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: LoginController.cs
    **
    ** Description:
    **      Login users
    **
    ** This program is free software: you can redistribute it and/or modify
    ** it under the terms of the GNU General Public License version 3 as
    ** published by the Free Software Foundation.   
    **
    ** This program is distributed in the hope that it will be useful,
    ** but WITHOUT ANY WARRANTY; without even the implied warranty of
    ** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    ** GNU General Public License version 3 for more details.
    **
    **  You should have received a copy of the GNU General Public License
    **  version 3 along with this program in file "license-gpl-3.0.txt".
    **  If not, see<http: //www.gnu.org/licenses/gpl-3.0.txt>.
    **
    **--------------------------------------------------------------------------*/


using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Notes2021Blazor.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Notes2021Blazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly NotesDbContext _db;

        public LoginController(IConfiguration configuration,
                               SignInManager<IdentityUser> signInManager,
                               UserManager<IdentityUser> userManager,
                               NotesDbContext db)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, login.RememberMe, false);

            if (!result.Succeeded) return BadRequest(new LoginResult { Successful = false, Error = "Username and password are invalid." });

            var user = await _signInManager.UserManager.FindByEmailAsync(login.Email);
            var roles = await _signInManager.UserManager.GetRolesAsync(user);

            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, login.Email));

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtAudience"],
                claims,
                expires: expiry,
                signingCredentials: creds
            );

            IdentityUser me = await _userManager.FindByEmailAsync(login.Email);
            UserData userData = _db.UserData.Single(p => p.UserId == me.Id);

            return Ok(new LoginResult { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token), Profile = userData });
        }
    }


    public class LoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class LoginResult
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public string Token { get; set; }
        public UserData Profile { get; set; }
    }
}