/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: FileController.cs
    **
    ** Description:
    **      Makes a database look like a file
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
using Microsoft.AspNetCore.Mvc;
using Notes2021Blazor.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Notes2021Blazor.Server.Controllers
{
    [Route("[controller]/{filename}")]
    public class FileController : Controller
    {
        private NotesDbContext _db;
        private IWebHostEnvironment hostingEnv;
     
        public FileController(NotesDbContext db, IWebHostEnvironment env)
        {
            this._db = db;
            this.hostingEnv = env;
        }

        [HttpGet]
        public async Task<FileResult> Get(string filename)
        {
            SQLFile myFile = await _db.SQLFile.SingleAsync(p => p.FileName == filename);

            FileResult mystuff = File((await (_db.SQLFileContent.SingleAsync(m => m.SQLFileId == myFile.FileId))).Content,
                System.Net.Mime.MediaTypeNames.Application.Octet,
                myFile.FileName);

            return mystuff;
        }

    }
}