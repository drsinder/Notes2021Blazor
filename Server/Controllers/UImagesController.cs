/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: UImagesController.cs
    **
    ** Description:
    **      Manage uploaded file (images mostly)
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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Http.Features;
using Notes2021Blazor.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Notes2021Blazor.Server.Controllers
{
    //[Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [Route("api/[controller]/{filename}")]
    [ApiController]
    public class UImagesController : ControllerBase
    {
        private NotesDbContext _db;
        private IWebHostEnvironment hostingEnv;

        public UImagesController(NotesDbContext db, IWebHostEnvironment env)
        {
            this._db = db;
            this.hostingEnv = env;
        }

        [HttpGet("[action]")]
        public async Task<List<SQLFile>> GetFiles()
        {
            return await _db.SQLFile.ToListAsync();
        }

        public async Task<List<SQLFileContent>> GetContent()
        {
            return await _db.SQLFileContent.ToListAsync();
        }

        [HttpPost("[action]")]
        public async Task Save(IList<IFormFile> UploadFiles)
        {
            try
            {
                foreach (IFormFile file in UploadFiles)
                {
                    var filename = ContentDispositionHeaderValue
                            .Parse(file.ContentDisposition)
                            .FileName
                            .Trim('"');

                    string nameonly = Path.GetFileNameWithoutExtension(filename);
                    string extonly = Path.GetExtension(filename);
                    string fname = nameonly + extonly;

                    SQLFile x = await _db.SQLFile.SingleOrDefaultAsync(p => p.FileName == fname);
                    if (x != null) // already exists
                    {
                        Response.Clear();
                        Response.StatusCode = 409;
                        Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File already exists";
                        return;
                    }

                    SQLFile SqlFile = new SQLFile();
                    SQLFileContent SqlFileContent = new SQLFileContent { Content = new byte[file.Length] };
                    using (Stream reader = file.OpenReadStream())
                    {
                        int xx = await reader.ReadAsync(SqlFileContent.Content, 0, (int)file.Length);
                        if (xx != file.Length)
                        {
                            Response.Clear();
                            Response.StatusCode = 204;
                            Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File failed to upload";
                            return;
                        }
                    }

                    SqlFile.ContentType = file.ContentType;
                    SqlFile.Contributor = "unknown";
                    SqlFile.FileName = filename;

                    _db.SQLFile.Add(SqlFile);
                    await _db.SaveChangesAsync();
                    long retId = SqlFile.FileId;

                    SqlFileContent.SQLFileId = retId;
                    _db.SQLFileContent.Add(SqlFileContent);
                    await _db.SaveChangesAsync();
                    retId = SqlFileContent.SQLFileId;

                    SqlFile.FileName = fname;
                    _db.Entry(SqlFile).State = EntityState.Modified;
                    await _db.SaveChangesAsync();


                    //filename = hostingEnv.ContentRootPath + "\\wwwroot" + $@"\{filename}";
                    //if (!System.IO.File.Exists(filename))
                    //{
                    //    using (FileStream fs = System.IO.File.Create(filename))
                    //    {
                    //        file.CopyTo(fs);
                    //        fs.Flush();
                    //    }
                    //}
                }
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.StatusCode = 204;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File failed to upload";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
        }
        [HttpPost("[action]")]
        public async Task Remove(IList<IFormFile> UploadFiles)
        {
            IFormFile file = UploadFiles[0];
            string filename = ContentDispositionHeaderValue
                .Parse(file.ContentDisposition)
                .FileName
                .Trim('"');

            string nameonly = Path.GetFileNameWithoutExtension(filename);
            string extonly = Path.GetExtension(filename);
            string fname = nameonly + extonly;

            SQLFile x = await _db.SQLFile.SingleOrDefaultAsync(p => p.FileName == fname);
            if (x == null)
                return;

            SQLFileContent y = await _db.SQLFileContent.SingleOrDefaultAsync(p => p.SQLFileId == x.FileId);
            _db.SQLFileContent.Remove(y);
            await _db.SaveChangesAsync();
            _db.SQLFile.Remove(x);
            await _db.SaveChangesAsync();

            Response.Clear();
            Response.StatusCode = 200;
            Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File removed successfully";


            //try
            //{
            //    var filename = hostingEnv.ContentRootPath + $@"\{UploadFiles[0].FileName}";
            //    if (System.IO.File.Exists(filename))
            //    {
            //        System.IO.File.Delete(filename);
            //    }
            //}
            //catch (Exception e)
            //{
            //    Response.Clear();
            //    Response.StatusCode = 200;
            //    Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File removed successfully";
            //    Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            //}
        }
    }
}