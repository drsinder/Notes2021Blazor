/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: AccountsController.cs
    **
    ** Description:
    **      Register users
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
using Notes2021Blazor.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Notes2021Blazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private static UserModel LoggedOutUser = new UserModel { IsAuthenticated = false };

        private readonly UserManager<IdentityUser> _userManager;
        private readonly NotesDbContext _db;

        public AccountsController(UserManager<IdentityUser> userManager, NotesDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]RegisterModel model)
        {
            var newUser = new IdentityUser { UserName = model.Email, Email = model.Email };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);

                return Ok(new RegisterResult { Successful = false, Errors = errors });
            }

            IdentityUser me = await _userManager.FindByEmailAsync(model.Email);
            UserData userData = new UserData
            {
                UserId = me.Id,
                DisplayName = model.DisplayName,
                TimeZoneID = Globals.TimeZoneDefaultID,
                Ipref2 = 12,
                Pref3 = true,
                MyGuid = Guid.NewGuid().ToString()
            };
            _db.UserData.Add(userData);

            // Add all new users to the User role
            await _userManager.AddToRoleAsync(newUser, "User");

            if (newUser.Email == Globals.PrimeAdminEmail)
                await _userManager.AddToRoleAsync(newUser, "Admin");


            await _db.SaveChangesAsync();

            return Ok(new RegisterResult { Successful = true });
        }
    }

    public class UserModel
    {
        public bool IsAuthenticated { get; set; }
        public UserData userData { get; set; }
    }

}