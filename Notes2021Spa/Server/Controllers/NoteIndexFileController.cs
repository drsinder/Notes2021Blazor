/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: NoteIndexFileController.cs
    **
    ** Description:
    **      Get a single note file
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notes2021Blazor.Shared;

namespace Notes2021Blazor.Server.Controllers
{
    [Authorize(Roles = "User,Guest")]
    [Route("api/[controller]")]
    [Route("api/[controller]/{sid}")]
    [ApiController]
    public class NoteIndexFileController : ControllerBase
    {
        private readonly NotesDbContext _db;

        public NoteIndexFileController(NotesDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<NoteFile> Get(string sid)
        {
            int nfid = int.Parse(sid);
            return _db.NoteFile.SingleOrDefault(p => p.Id == nfid);
        }
    }
}