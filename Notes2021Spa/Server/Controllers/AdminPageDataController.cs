/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: AdminPageDataController.cs
    **
    ** Description:
    **      
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

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Notes2021Blazor.Shared;
using Notes2021Blazor.Shared.Manager;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Notes2021Blazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminPageDataController : ControllerBase
    {
        private readonly NotesDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminPageDataController(NotesDbContext db,
            UserManager<IdentityUser> userManager
            )
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<HomePageModel> Get()
        {
            HomePageModel model = new HomePageModel();

            //model.Message = _db.HomePageMessage.FirstOrDefault();

            NoteFile hpmf = _db.NoteFile.Where(p => p.NoteFileName == "homepagemessages").FirstOrDefault();
            if (hpmf != null)
            {
                NoteHeader hpmh = _db.NoteHeader.Where(p => p.NoteFileId == hpmf.Id).OrderByDescending(p => p.CreateDate).FirstOrDefault();
                if (hpmh != null)
                {
                    model.Message = _db.NoteContent.Where(p => p.NoteHeaderId == hpmh.Id).FirstOrDefault().NoteBody;
                }
            }

            model.NoteFiles = _db.NoteFile
                .OrderBy(p => p.NoteFileName).ToList();

            model.NoteAccesses = new List<NoteAccess>();

            List<UserData> udl = _db.UserData.ToList();

            foreach (NoteFile nf in model.NoteFiles)
            {
                UserData ud = udl.Find(p => p.UserId == nf.OwnerId);
                ud.MyGuid = "";
                ud.MyStyle = "";
                nf.Owner = ud;
            }

            try
            {
                string userName = this.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                if (!string.IsNullOrEmpty(userName))
                {
                    IdentityUser user = await _userManager.FindByNameAsync(userName);
                    model.UserData = _db.UserData.Single(p => p.UserId == user.Id);

                    foreach (NoteFile nf in model.NoteFiles)
                    {
                        NoteAccess na = await AccessManager.GetAccess(_db, user.Id, nf.Id, 0);
                        model.NoteAccesses.Add(na);
                    }

                    //if (model.NoteAccesses.Count > 0)
                    //{
                    //    NoteFile[] theList = new NoteFile[model.NoteFiles.Count];
                    //    model.NoteFiles.CopyTo(theList);
                    //    foreach (NoteFile nf2 in theList)
                    //    {
                    //        NoteAccess na = model.NoteAccesses.SingleOrDefault(p => p.NoteFileId == nf2.Id);
                    //        if (!na.ReadAccess && !na.Write && !na.EditAccess)
                    //        {
                    //            model.NoteFiles.Remove(nf2);
                    //        }
                    //    }
                    //}
                }
                else
                {
                    model.UserData = new UserData { TimeZoneID = Globals.TimeZoneDefaultID };
                }
            }
            catch
            {
                model.UserData = new UserData { TimeZoneID = Globals.TimeZoneDefaultID };
            }

            model.TimeZone = _db.TZone.Single(p => p.Id == model.UserData.TimeZoneID);

            return model;
        }
    }
}