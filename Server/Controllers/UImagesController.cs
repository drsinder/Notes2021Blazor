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

        //[HttpGet("[action]")]
        //public async Task<FileResult> Get(string filename)
        //{
        //    SQLFile myFile = await _db.SQLFile.SingleAsync(p => p.FileName == filename);

        //    FileResult mystuff = File((await (_db.SQLFileContent.SingleAsync(m => m.SQLFileId == myFile.FileId))).Content,
        //        System.Net.Mime.MediaTypeNames.Application.Octet,
        //        myFile.FileName);

        //    return mystuff;
        //}


        [HttpPost("[action]")]
        public async Task Save(IList<IFormFile> UploadFiles)
        {
            //long size = 0;
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

                    SQLFile SqlFile = new SQLFile();
                    SQLFileContent SqlFileContent = new SQLFileContent { Content = new byte[file.Length] };
                    using (Stream reader = file.OpenReadStream())
                    {
                        int x = await reader.ReadAsync(SqlFileContent.Content, 0, (int)file.Length);
                        if (x != file.Length)
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

                    string fname = nameonly + extonly;
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
        public void Remove(IList<IFormFile> UploadFiles)
        {
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