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
    [Authorize(Roles = "User")]
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