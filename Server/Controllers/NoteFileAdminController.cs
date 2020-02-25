/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: NoteFileAdminController.cs
    **
    ** Description:
    **      Create, delete, edit, and get notes files (admin)
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
using Notes2021Blazor.Shared;
using Notes2021Blazor.Shared.Manager;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes2021Blazor.Server.Controllers
{
    [Authorize(Roles = "User,Guest")]
    [Route("api/[controller]")]
    [Route("api/[controller]/{id}")]
    [ApiController]
    public class NoteFileAdminController : ControllerBase
    {
        private readonly NotesDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public NoteFileAdminController(NotesDbContext db,
                UserManager<IdentityUser> userManager
                )
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<List<NoteFile>> Get()
        {
            return await NoteDataManager.GetNoteFilesOrderedByName(_db);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task Post(CreateFileModel crm)
        {
            IdentityUser me = await _userManager.FindByNameAsync(User.Identity.Name);

            await NoteDataManager.CreateNoteFile(_db, _userManager, me.Id, crm.NoteFileName, crm.NoteFileTitle);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task Delete(string id)
        {
            int intid = int.Parse(id);
            await NoteDataManager.DeleteNoteFile(_db, intid);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task Put(NoteFile edited)
        {
            NoteFile live = await _db.NoteFile.FindAsync(edited.Id);

            live.LastEdited = DateTime.Now.ToUniversalTime();
            live.NoteFileName = edited.NoteFileName;
            live.NoteFileTitle = edited.NoteFileTitle;
            live.OwnerId = edited.OwnerId;

            _db.Update(live);
            await _db.SaveChangesAsync();
        }
    }
}