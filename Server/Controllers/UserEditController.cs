using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Notes2021Blazor.Shared;

namespace Notes2021Blazor.Server.Controllers
{
    [Route("api/[controller]")]
    [Route("api/[controller]/{Id}")]
    [ApiController]
    public class UserEditController : ControllerBase
    {
        private readonly NotesDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public UserEditController(NotesDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<EditUserViewModel> Get(string Id)
        {
            UserData me = await _db.UserData.SingleOrDefaultAsync(p => p.UserId == Id);

            IdentityUser user = await _userManager.FindByIdAsync(Id);

            var myRoles = await _userManager.GetRolesAsync(user);

            List<IdentityRole> allRoles = _db.Roles.OrderBy(p => p.Name).ToList();

            List<CheckedUser> myList = new List<CheckedUser>();

            foreach (IdentityRole item in allRoles)
            {
                CheckedUser it = new CheckedUser();
                it.theRole = item;
                it.isMember = myRoles.Where(p => p == item.Name).FirstOrDefault() != null;
                myList.Add(it);
            }

            EditUserViewModel stuff =  new EditUserViewModel()
            {
                UserData = me,
                RolesList = myList
            };

            return stuff;
        }

        [HttpPut]
        public async Task Put(EditUserViewModel model)
        {
            IdentityUser user = await _userManager.FindByIdAsync(model.UserData.UserId);
            var myRoles = await _userManager.GetRolesAsync(user);
            foreach(CheckedUser item in model.RolesList)
            {
                if (item.isMember && !myRoles.Contains(item.theRole.Name)) // need to add role
                {
                    await _userManager.AddToRoleAsync(user, item.theRole.Name);
                }
                else if (!item.isMember && myRoles.Contains(item.theRole.Name)) // need to remove role
                {
                    await _userManager.RemoveFromRoleAsync(user, item.theRole.Name);
                }
            }
        }

    }
}