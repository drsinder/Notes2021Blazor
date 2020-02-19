/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: AccessListController.cs
    **
    ** Description:
    **      Access list managment
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
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Notes2021Blazor.Server.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]/{fileId}")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccessListController : ControllerBase
    {
        private readonly NotesDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public AccessListController(NotesDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<List<NoteAccess>> Get(string fileId)
        {
            int Id = int.Parse(fileId);

            List<NoteAccess> list = await _db.NoteAccess.Where(p => p.NoteFileId == Id).OrderBy(p => p.ArchiveId).ToListAsync();

            return list;
        }

        [HttpPut]
        public async Task Put(NoteAccess item)
        {
            IdentityUser me = await _userManager.FindByNameAsync(User.Identity.Name);
            NoteAccess myAccess = await AccessManager.GetAccess(_db, me.Id, item.NoteFileId, item.ArchiveId);
            if (!myAccess.EditAccess)
                return;

            NoteAccess work = await _db.NoteAccess.Where(p => p.NoteFileId == item.NoteFileId
                && p.ArchiveId == item.ArchiveId && p.UserID == item.UserID)
                .FirstOrDefaultAsync();
            if (work == null)
                return;

            work.ReadAccess = item.ReadAccess;
            work.Respond = item.Respond;
            work.Write = item.Write;
            work.DeleteEdit = item.DeleteEdit;
            work.SetTag = item.SetTag;
            work.ViewAccess = item.ViewAccess;
            work.EditAccess = item.EditAccess;

            _db.Update(work);
            await _db.SaveChangesAsync();
        }

        [HttpPost]
        public async Task Post(NoteAccess item)
        {
            IdentityUser me = await _userManager.FindByNameAsync(User.Identity.Name);
            NoteAccess myAccess = await AccessManager.GetAccess(_db, me.Id, item.NoteFileId, item.ArchiveId);
            if (!myAccess.EditAccess)
                return;

            NoteAccess work = await _db.NoteAccess.Where(p => p.NoteFileId == item.NoteFileId
                && p.ArchiveId == item.ArchiveId && p.UserID == item.UserID)
                .FirstOrDefaultAsync();
            if (work != null)
                return;     // already exists

            if (item.UserID == Globals.AccessOtherId())
                return;     // can not create "Other"

            NoteFile nf = _db.NoteFile.Where(p => p.Id == item.NoteFileId).FirstOrDefault();

            if (item.ArchiveId < 0 || item.ArchiveId > nf.NumberArchives)
                return;

            _db.NoteAccess.Add(item);
            await _db.SaveChangesAsync();
        }

        [HttpDelete]
        public async Task Delete(string fileId)
        {
            string[] parts = fileId.Split(".");
            if (parts.Length != 3)
                return;

            string uid = parts[2];
            int fid = int.Parse(parts[0]);
            int aid = int.Parse(parts[1]);

            if (uid == Globals.AccessOtherId())
                return;     // can not delete "Other"

            // also can not delete self
            string userName = this.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            IdentityUser user = await _userManager.FindByNameAsync(userName);
            NoteAccess myAccess = await AccessManager.GetAccess(_db, user.Id, fid, aid);
            if (!myAccess.EditAccess)
                return;

            if (uid == user.Id)
                return;     // can not delete self"

            NoteAccess work = await _db.NoteAccess.Where(p => p.NoteFileId == fid
                && p.ArchiveId == aid && p.UserID == uid)
                .FirstOrDefaultAsync();
            if (work == null)
                return;

            _db.NoteAccess.Remove(work);
            await _db.SaveChangesAsync();

        }

    }
}