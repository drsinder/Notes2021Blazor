﻿/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: LocalService.cs
    **
    ** Description:
    **      Makes content from note to email
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


using Microsoft.EntityFrameworkCore;
using Notes2021Blazor.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notes2021Blazor.Shared.Manager;

namespace Notes2021Blazor.Server.Services
{
    public static class LocalService
    {
        public static async Task<string> MakeNoteForEmail(ForwardViewModel fv, NotesDbContext db, string email, string name)
        {
            NoteHeader nc = await NoteDataManager.GetNoteByIdWithFile(db, fv.NoteID);

            if (!fv.hasstring || !fv.wholestring)
            {
                return "Forwarded by Notes 2021 - User: " + email + " / " + name
                    + "<p>File: " + nc.NoteFile.NoteFileName + " - File Title: " + nc.NoteFile.NoteFileTitle + "</p><hr/>"
                    + "<p>Author: " + nc.AuthorName + "  - Director Message: " + nc.NoteContent.DirectorMessage + "</p><p>"
                    + "<p>Subject: " + nc.NoteSubject + "</p>"
                    + nc.LastEdited.ToShortDateString() + " " + nc.LastEdited.ToShortTimeString() + " UTC" + "</p>"
                    + nc.NoteContent.NoteBody
                    + "<hr/>" + "<a href=\"" + Globals.ProductionUrl + "?show=" + fv.NoteID + "\" >Link to note</a>";
            }
            else
            {
                List<NoteHeader> bnhl = await db.NoteHeader
                    .Where(p => p.NoteFileId == nc.NoteFileId && p.NoteOrdinal == nc.NoteOrdinal && p.ResponseOrdinal == 0)
                    .ToListAsync();
                NoteHeader bnh = bnhl[0];
                fv.NoteSubject = bnh.NoteSubject;
                List<NoteHeader> notes = await db.NoteHeader.Include("NoteContent")
                    .Where(p => p.NoteFileId == nc.NoteFileId && p.NoteOrdinal == nc.NoteOrdinal)
                    .ToListAsync();

                StringBuilder sb = new StringBuilder();
                sb.Append("Forwarded by Notes 2020 - User: " + email + " / " + name
                    + "<p>\nFile: " + nc.NoteFile.NoteFileName + " - File Title: " + nc.NoteFile.NoteFileTitle + "</p>"
                    + "<hr/>");

                for (int i = 0; i < notes.Count; i++)
                {
                    if (i == 0)
                    {
                        sb.Append("<p>Base Note - " + (notes.Count - 1) + " Response(s)</p>");
                    }
                    else
                    {
                        sb.Append("<hr/><p>Response - " + notes[i].ResponseOrdinal + " of " + (notes.Count - 1) + "</p>");
                    }
                    sb.Append("<p>Author: " + notes[i].AuthorName + "  - Director Message: " + notes[i].NoteContent.DirectorMessage + "</p>");
                    sb.Append("<p>Subject: " + notes[i].NoteSubject + "</p>");
                    sb.Append("<p>" + notes[i].LastEdited.ToShortDateString() + " " + notes[i].LastEdited.ToShortTimeString() + " UTC" + " </p>");
                    sb.Append(notes[i].NoteContent.NoteBody);
                    sb.Append("<hr/>");
                    sb.Append("<a href=\"");
                    sb.Append(Globals.ProductionUrl + "show=" + notes[i].Id + "\" >Link to note</a>");
                }

                return sb.ToString();
            }
        }
    }
}
