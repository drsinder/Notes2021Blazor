/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: LocalManager.cs
    **
    ** Description:
    **      Get various items
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


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Notes2021Blazor.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Notes2021Blazor.Server
{
    public static class LocalManager
    {
        public static UserData GetUserData(string userid, NotesDbContext db)
        {
            UserData aux = null;
            try
            {
                aux = db.UserData.SingleOrDefault(p => p.UserId == userid);
            }
            catch
            { }
            return aux;
        }

        public static async Task<TZone> GetUserTimeZone(string uid,
            NotesDbContext db)
        {
            int tzid = Globals.TimeZoneDefaultID;
            try
            {
                tzid = GetUserData(uid, db).TimeZoneID;  // get users timezoneid
            }
            catch
            {
                // ignored
            }
            if (tzid < 1)
                tzid = Globals.TimeZoneDefaultID;

            var tz2 = await db.TZone.SingleAsync(p => p.Id == tzid);

            return tz2;
        }


        public static IEnumerable<SelectListItem> GetFileNameSelectList(NotesDbContext db)
        {

            // Get a list of all files for dropdowns by name
            return db.NoteFile
                .OrderBy(c => c.NoteFileName)
                .Select(c => new SelectListItem
                {
                    Value = c.NoteFileName,
                    Text = c.NoteFileName
                });
        }

        public static IEnumerable<SelectListItem> GetFileTitleSelectList(NotesDbContext db)
        {

            // Get a list of all files for dropdowns by title
            return db.NoteFile
                .OrderBy(c => c.NoteFileTitle)
                .Select(c => new SelectListItem
                {
                    Value = c.NoteFileName,
                    Text = c.NoteFileTitle
                });
        }

        public static IEnumerable<SelectListItem> GetFileNameSelectListWithId(NotesDbContext db)
        {

            // Get a list of all files for dropdowns by name
            return db.NoteFile
                .OrderBy(c => c.NoteFileName)
                .Select(c => new SelectListItem
                {
                    Value = "" + $"{c.Id}",
                    Text = c.NoteFileName
                });
        }



    }

}
