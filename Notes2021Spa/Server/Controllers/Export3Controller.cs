/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: Export3Controller.cs
    **
    ** Description:
    **      Supply data for export
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
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes2021Blazor.Server.Services;
using Notes2021Blazor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Notes2021Blazor.Server.Controllers
{
    [Authorize(Roles = "User,Guest")]
    [Route("api/[controller]")]
    [Route("api/[controller]/{modelstring}")]
    [ApiController]
    public class Export3Controller : ControllerBase
    {
        private readonly NotesDbContext _db;

        public Export3Controller(NotesDbContext db )
        {
            _db = db;
        }

        [HttpGet]
        public async Task<List<NoteHeader>> Get(string modelstring)
        {
            int arcId;
            int fileId;
            int noteOrd;

            string[] parts = modelstring.Split(".");

            fileId = int.Parse(parts[0]);
            arcId = int.Parse(parts[1]);
            noteOrd = int.Parse(parts[2]);

            List<NoteHeader> nhl = await _db.NoteHeader
                .Include(m => m.NoteContent)
                .Include(m => m.Tags)
                .Where(p => p.NoteFileId == fileId && p.ArchiveId == arcId && p.NoteOrdinal == noteOrd && p.ResponseOrdinal > 0)
                .OrderBy(p => p.ResponseOrdinal)
                .ToListAsync();

            return nhl;
        }

    }
}