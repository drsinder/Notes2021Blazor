/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: MyAccessController.cs
    **
    ** Description:
    **      Get access item for login user
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
using Notes2021Blazor.Shared;
using System.Linq;
using System.Threading.Tasks;

namespace Notes2021Blazor.Server.Controllers
{
    [Authorize(Roles = "User,Guest")]
    [Route("api/[controller]/{fileId}")]
    [ApiController]
    public class MyAccessController : ControllerBase
    {
        private readonly NotesDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public MyAccessController(NotesDbContext db,
            UserManager<IdentityUser> userManager
            )
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<NoteAccess> Get(string fileId)
        {
            int Id = int.Parse(fileId);

            IdentityUser me = await _userManager.FindByNameAsync(User.Identity.Name);

            NoteAccess mine = await _db.NoteAccess.Where(p => p.NoteFileId == Id && p.UserID == me.Id && p.ArchiveId == 0).OrderBy(p => p.ArchiveId).FirstOrDefaultAsync();

            if (mine == null)
            {
                mine = await _db.NoteAccess.Where(p => p.NoteFileId == Id && p.UserID == Globals.AccessOtherId() && p.ArchiveId == 0).OrderBy(p => p.ArchiveId).FirstOrDefaultAsync();
            }

            return mine;
        }

    }
}