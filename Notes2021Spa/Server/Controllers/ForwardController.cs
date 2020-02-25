/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: ForwardController.cs
    **
    ** Description:
    **      Forward a note by email
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


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes2021Blazor.Server.Services;
using Notes2021Blazor.Shared;
using Notes2021Blazor.Shared.Manager;
using System.Threading.Tasks;

namespace Notes2021Blazor.Server.Controllers
{
    [Authorize(Roles = "User,Guest")]
    [Route("api/[controller]")]
    [ApiController]
    public class ForwardController : ControllerBase
    {
        private readonly NotesDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public ForwardController(NotesDbContext db,
            UserManager<IdentityUser> userManager
          )
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task Post(ForwardViewModel fv)
        {
            NoteHeader nh = await NoteDataManager.GetBaseNoteHeaderById(_db, fv.NoteID);

            IdentityUser usr = await _userManager.FindByNameAsync(User.Identity.Name);

            UserData ud = await _db.UserData.SingleOrDefaultAsync(p => p.UserId == usr.Id);

            string myEmail = await LocalService.MakeNoteForEmail(fv, _db, User.Identity.Name, ud.DisplayName);

            EmailSender emailSender = new EmailSender();

            await emailSender.SendEmailAsync(usr.UserName, fv.NoteSubject, myEmail);
        }

    }
}