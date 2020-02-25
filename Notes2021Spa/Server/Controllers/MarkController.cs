/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: MarkController.cs
    **
    ** Description:
    **      Mark managment
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
using Notes2021Blazor.Shared.Manager;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes2021Blazor.Server.Controllers
{
    [Authorize(Roles = "User,Guest")]
    [Route("api/[controller]")]
    [Route("api/[controller]/{deleteId}")]

    [ApiController]
    public class MarkController : ControllerBase
    {
        private readonly NotesDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public MarkController(NotesDbContext db,
                UserManager<IdentityUser> userManager
                )
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<List<Mark>> Get()
        {
            UserData me = NoteDataManager.GetUserData(_userManager, User, _db);

            List<Mark> list = await _db.Mark.Where(p => p.UserId == me.UserId).ToListAsync();
            return list;
        }

        [HttpPost]
        public async Task Post(Mark mrk)
        {
            UserData me = NoteDataManager.GetUserData(_userManager, User, _db);

            mrk.UserId = me.UserId;

            Mark lastMark = await _db.Mark.Where(p => p.UserId == me.UserId)
                .OrderByDescending(p => p.MarkOrdinal)
                .FirstOrDefaultAsync();

            if (lastMark == null || lastMark.MarkOrdinal == 0)
                mrk.MarkOrdinal = 1;
            else
                mrk.MarkOrdinal = lastMark.MarkOrdinal + 1;

            _db.Mark.Add(mrk);
            await _db.SaveChangesAsync();
        }


        [HttpPut]
        public async Task Put(List<Mark> mrks)
        {
            UserData me = NoteDataManager.GetUserData(_userManager, User, _db);
            int lastNum;

            Mark lastMark = await _db.Mark.Where(p => p.UserId == me.UserId)
                .OrderByDescending(p => p.MarkOrdinal)
                .FirstOrDefaultAsync();

            if (lastMark == null || lastMark.MarkOrdinal == 0)
                lastNum = 1;
            else
                lastNum = lastMark.MarkOrdinal + 1;


            List<Mark> list = await _db.Mark.Where(p => p.UserId == me.UserId).ToListAsync();

            foreach (Mark mrk in mrks)
            {
                mrk.UserId = me.UserId;
                mrk.MarkOrdinal = lastNum++;
                _db.Mark.Add(mrk);
            }

            await _db.SaveChangesAsync();
        }

        [HttpDelete]
        public async Task Delete(string deleteId)
        {
            string[] stuff = deleteId.Split(".");

            int fileId = int.Parse(stuff[0]);
            int arcId = int.Parse(stuff[1]);
            UserData me = NoteDataManager.GetUserData(_userManager, User, _db);
            List<Mark> list = await _db.Mark.Where(p => p.UserId == me.UserId && p.NoteFileId == fileId && p.ArchiveId == arcId).ToListAsync();

            _db.RemoveRange(list);
            await _db.SaveChangesAsync();
        }
    }
}