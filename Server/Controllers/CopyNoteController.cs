using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Notes2021Blazor.Shared;
using Notes2021Blazor.Shared.Manager;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace Notes2021Blazor.Server.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class CopyNoteController : ControllerBase
    {
        private readonly NotesDbContext _db;
        private NoteFile noteFile;
        public CopyNoteController(NotesDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task Post(CopyModel Model)
        {
            int fileId = Model.FileId;

            string uid = Model.UserData.UserId;
            NoteAccess myAccess = await AccessManager.GetAccess(_db, uid, fileId, 0);
            if (!myAccess.Write)
                return;

            NoteHeader Header = Model.Note;
            bool whole = Model.WholeString;
            noteFile = await _db.NoteFile.SingleAsync(p => p.Id == fileId);

            if (!whole)
            {
                NoteContent cont = await _db.NoteContent.SingleAsync(p => p.NoteHeaderId == Header.Id);
                //cont.NoteHeader = null;
                List<Tags> tags = await _db.Tags.Where(p => p.NoteHeaderId == Header.Id).ToListAsync();

                string Body = string.Empty;
                Body = MakeHeader(Header);
                Body += cont.NoteBody;

                Header = Header.CloneForLink();

                Header.Id = 0;
                Header.ArchiveId = 0;
                Header.LinkGuid = string.Empty;
                Header.NoteOrdinal = 0;
                Header.ResponseCount = 0;
                Header.NoteFileId = fileId;
                Header.BaseNoteId = 0;
                Header.NoteFile = null;
                Header.AuthorID = Model.UserData.UserId;
                Header.AuthorName = Model.UserData.DisplayName;

                Header.CreateDate = Header.ThreadLastEdited = Header.LastEdited = DateTime.Now.ToUniversalTime();

                await NoteDataManager.CreateNote(_db, null, Header, Body, Tags.ListToString(tags), cont.DirectorMessage, true, false);

                return;
            }
            else
            {
                // get base note

                NoteHeader BaseHeader;
                BaseHeader = await _db.NoteHeader.SingleAsync(p => p.NoteFileId == Header.NoteFileId
                    && p.ArchiveId == Header.ArchiveId
                    && p.NoteOrdinal == Header.NoteOrdinal
                    && p.ResponseOrdinal == 0);

                Header = BaseHeader.CloneForLink();

                NoteContent cont = await _db.NoteContent.SingleAsync(p => p.NoteHeaderId == Header.Id);
                //cont.NoteHeader = null;
                List<Tags> tags = await _db.Tags.Where(p => p.NoteHeaderId == Header.Id).ToListAsync();

                string Body = string.Empty;
                Body = MakeHeader(Header);
                Body += cont.NoteBody;

                Header.Id = 0;
                Header.ArchiveId = 0;
                Header.LinkGuid = string.Empty;
                Header.NoteOrdinal = 0;
                Header.ResponseCount = 0;
                Header.NoteFileId = fileId;
                Header.BaseNoteId = 0;
                Header.NoteFile = null;
                Header.AuthorID = Model.UserData.UserId;
                Header.AuthorName = Model.UserData.DisplayName;

                Header.CreateDate = Header.ThreadLastEdited = Header.LastEdited = DateTime.Now.ToUniversalTime();

                Header.NoteContent = null;

                NoteHeader NewHeader = await NoteDataManager.CreateNote(_db, null, Header, Body, Tags.ListToString(tags), cont.DirectorMessage, true, false);

                for (int i = 1; i <= BaseHeader.ResponseCount; i++)
                {
                    NoteHeader RHeader = await _db.NoteHeader.SingleAsync(p => p.NoteFileId == BaseHeader.NoteFileId
                        && p.ArchiveId == BaseHeader.ArchiveId
                        && p.NoteOrdinal == BaseHeader.NoteOrdinal
                        && p.ResponseOrdinal == i);

                    Header = RHeader.CloneForLinkR();

                    cont = await _db.NoteContent.SingleAsync(p => p.NoteHeaderId == Header.Id);
                    tags = await _db.Tags.Where(p => p.NoteHeaderId == Header.Id).ToListAsync();

                    Body = string.Empty;
                    Body = MakeHeader(Header);
                    Body += cont.NoteBody;

                    Header.Id = 0;
                    Header.ArchiveId = 0;
                    Header.LinkGuid = string.Empty;
                    Header.NoteOrdinal = NewHeader.NoteOrdinal;
                    Header.ResponseCount = 0;
                    Header.NoteFileId = fileId;
                    Header.BaseNoteId = NewHeader.Id;
                    Header.NoteFile = null;
                    Header.ResponseOrdinal = 0;
                    Header.AuthorID = Model.UserData.UserId;
                    Header.AuthorName = Model.UserData.DisplayName;

                    Header.CreateDate = Header.ThreadLastEdited = Header.LastEdited = DateTime.Now.ToUniversalTime();

                    await NoteDataManager.CreateResponse(_db, null, Header, Body, Tags.ListToString(tags), cont.DirectorMessage, true, false);
                }

            }
        }

        private string MakeHeader(NoteHeader header)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<div class=\"copiednote\">From: ");
            sb.Append(noteFile.NoteFileName);
            sb.Append(" - ");
            sb.Append(header.NoteSubject);
            sb.Append(" - ");
            sb.Append(header.AuthorName);
            sb.Append(" - ");
            sb.Append(header.CreateDate.ToShortDateString());
            sb.AppendLine("</div>");
            return sb.ToString();
        }
    }
}