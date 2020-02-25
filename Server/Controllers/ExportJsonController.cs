/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: ExportController.cs
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
    [Route("api/[controller]/{modelstring}")]
    [ApiController]
    public class ExportJsonController : ControllerBase
    {
        private readonly NotesDbContext _db;

        public ExportJsonController(NotesDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<JsonExport> Get(string modelstring)
        {
            int arcId;
            int fileId;

            string[] parts = modelstring.Split(".");

            fileId = int.Parse(parts[0]);
            arcId = int.Parse(parts[1]);

            JsonExport stuff = new JsonExport();

            stuff.NoteFile = _db.NoteFile.Single(p => p.Id == fileId);

            stuff.NoteHeaders = await _db.NoteHeader
                    .Where(p => p.NoteFileId == fileId && p.ArchiveId == arcId)
                    .OrderBy(p => p.NoteOrdinal)
                    .ThenBy(p => p.ResponseOrdinal)
                    .ToListAsync();

            List<long> Ids = new List<long>();
            foreach (NoteHeader item in stuff.NoteHeaders)
            {
                Ids.Add(item.Id);
            }

            stuff.NoteContents = await _db.NoteContent
                    .Where(p => Ids.Contains(p.NoteHeaderId))
                    .ToListAsync();

            foreach (NoteContent item in stuff.NoteContents)
            {
                item.NoteHeader = null;
            }

            stuff.Tags = await _db.Tags
                .Where(p => Ids.Contains(p.NoteHeaderId))
                .ToListAsync();

            return stuff;
        }

    }
}