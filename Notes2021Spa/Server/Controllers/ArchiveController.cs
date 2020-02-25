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
    public class ArchiveController : ControllerBase
    {
        private readonly NotesDbContext _db;

        public ArchiveController(NotesDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<List<NoteHeader>> Get(string sid)
        {
            int fileid = int.Parse(sid);
            List<NoteHeader> list = new List<NoteHeader>();

            NoteFile noteFile = _db.NoteFile.SingleOrDefault(p => p.Id == fileid);

            for ( int i = 0; i <= noteFile.NumberArchives; i++)
            {
                NoteHeader header = _db.NoteHeader.Where(p => p.ArchiveId == i).OrderBy(p => p.CreateDate).FirstOrDefault();
                list.Add(header);
            }
        
            return list;
        }

    }
}