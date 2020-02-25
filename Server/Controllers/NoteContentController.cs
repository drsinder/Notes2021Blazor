/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: NoteContentController.cs
    **
    ** Description:
    **      Gets note content and tags
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes2021Blazor.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes2021Blazor.Server.Controllers
{
    [Authorize(Roles = "User,Guest")]
    [Route("api/[controller]")]
    [Route("api/[controller]/{sid}")]
    [ApiController]
    public class NoteContentController : ControllerBase
    {
        private readonly NotesDbContext _db;

        public NoteContentController(NotesDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<DisplayModel> Get(string sid)
        {
            long id = long.Parse(sid);

            NoteContent c = _db.NoteContent.Single(p => p.NoteHeaderId == id);
            List<Tags> tags = await _db.Tags.Where(p => p.NoteHeaderId == id).ToListAsync();

            return new DisplayModel { content = c, tags = tags };
        }

    }
}