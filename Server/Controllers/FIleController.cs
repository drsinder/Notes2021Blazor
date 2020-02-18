using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Notes2021Blazor.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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