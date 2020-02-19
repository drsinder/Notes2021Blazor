/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: HomePageMessageController.cs
    **
    ** Description:
    **      Manage home page messages
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
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class HomePageMessageController : ControllerBase
    {
        private readonly NotesDbContext _db;

        public HomePageMessageController(NotesDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<HomePageMessage> Get()
        {
            HomePageMessage x = _db.HomePageMessage.FirstOrDefault();
            if (x == null)
            {
                x = new HomePageMessage();
                x.Id = 0;
                x.Message = string.Empty;
            }

            return x;
        }

        [HttpDelete]
        public async Task Delete()
        {
            List<HomePageMessage> mine = _db.HomePageMessage.ToList();

            if (mine == null || mine.Count == 0)
                return;

            _db.HomePageMessage.RemoveRange(mine);
            await _db.SaveChangesAsync();
        }

        [HttpPost]
        public async Task Post(HomePageMessage mess)
        {
            mess.Posted = DateTime.Now.ToUniversalTime();

            _db.HomePageMessage.Add(mess);
            await _db.SaveChangesAsync();
        }

        [HttpPut]
        public async Task Put(HomePageMessage mess)
        {
            mess.Posted = DateTime.Now.ToUniversalTime();

            _db.Entry(mess).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _db.SaveChangesAsync();
        }
    }
}