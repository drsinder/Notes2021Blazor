using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes2021Blazor.Shared;
using System.Threading.Tasks;

namespace Notes2021Blazor.Server.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class NoteFileAdminStdController : ControllerBase
    {
        private readonly NotesDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public NoteFileAdminStdController(NotesDbContext db,
                UserManager<IdentityUser> userManager
                )
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task Post(Stringy file)
        {
            switch (file.value)
            {
                case "announce":
                    await CreateAnnounce();
                    break;

                case "pbnotes":
                    await CreatePbnotes();
                    break;

                case "noteshelp":
                    await CreateNoteshelp();
                    break;

                case "pad":
                    await CreatePad();
                    break;

                default:
                    break;
            }
        }

        public async Task<bool> CreateNoteFile(string name, string title)
        {
            IdentityUser me = await _userManager.FindByNameAsync(User.Identity.Name);

            return await NoteDataManager.CreateNoteFile(_db, _userManager, me.Id, name, title);
        }

        public async Task CreateAnnounce()
        {
            await CreateNoteFile("announce", "Notes 2021 Announcements");
            NoteFile nf4 = await NoteDataManager.GetFileByName(_db, "announce");
            int padid = nf4.Id;
            NoteAccess access = await AccessManager.GetOneAccess(_db, Globals.AccessOtherId(), padid, 0);
            access.ReadAccess = true;

            _db.Entry(access).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task CreatePbnotes()
        {
            await CreateNoteFile("pbnotes", "Public Notes");
            NoteFile nf4 = await NoteDataManager.GetFileByName(_db, "pbnotes");
            int padid = nf4.Id;
            NoteAccess access = await AccessManager.GetOneAccess(_db, Globals.AccessOtherId(), padid, 0);
            access.ReadAccess = true;
            access.Respond = true;
            access.Write = true;

            _db.Entry(access).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task CreateNoteshelp()
        {
            await CreateNoteFile("noteshelp", "Help with Notes 2021");
            NoteFile nf4 = await NoteDataManager.GetFileByName(_db, "noteshelp");
            int padid = nf4.Id;
            NoteAccess access = await AccessManager.GetOneAccess(_db, Globals.AccessOtherId(), padid, 0);
            access.ReadAccess = true;
            access.Respond = true;
            access.Write = true;

            _db.Entry(access).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task CreatePad()
        {
            await CreateNoteFile("pad", "Traditional Pad");
            NoteFile nf4 = await NoteDataManager.GetFileByName(_db, "pad");
            int padid = nf4.Id;
            NoteAccess access = await AccessManager.GetOneAccess(_db, Globals.AccessOtherId(), padid, 0);
            access.ReadAccess = true;
            access.Respond = true;
            access.Write = true;

            _db.Entry(access).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

    }
}