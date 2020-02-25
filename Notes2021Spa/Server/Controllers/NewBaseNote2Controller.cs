﻿/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: NewBaseNote2Controller.cs
    **
    ** Description:
    **      Get a note header for most recent note created
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

using Microsoft.AspNetCore.Mvc;
using Notes2021Blazor.Shared;
using System.Linq;
using System.Threading.Tasks;

namespace Notes2021Blazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewBaseNote2Controller : ControllerBase
    {
        private readonly NotesDbContext _db;

        public NewBaseNote2Controller(NotesDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<NoteHeader> Get()
        {
            NoteHeader nh = _db.NoteHeader.OrderByDescending(p => p.Id).FirstOrDefault();
            return nh;
        }
    }
}
