/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: TimeZonesController.cs
    **
    ** Description:
    **      Get list of time zones
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
using Microsoft.EntityFrameworkCore;
using Notes2021Blazor.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes2021Blazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeZonesController : ControllerBase
    {
        private readonly NotesDbContext _db;

        public TimeZonesController(NotesDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<List<TZone>> Get()
        {
            List<TZone> list = await _db.TZone.ToListAsync();

            return list;
        }

    }
}