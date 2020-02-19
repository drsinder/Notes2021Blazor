/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: ApiLinkRController.cs
    **
    ** Description:
    **      Linked file managment
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notes2021Blazor.Shared;

namespace Notes2021Blazor.Server.Controllers
{
    /// <summary>
    /// Has functions from former ApiLinkR,  Controllers
    /// </summary>

    [Route("api/[controller]")]
    [Route("api/[controller]/{file}")]
    [ApiController]
    public class ApiLinkRController : ControllerBase
    {
        private readonly NotesDbContext _context;

        public ApiLinkRController(NotesDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<string> CreateLinkResponse(LinkCreateRModel inputModel)
        {
            NoteFile file = _context.NoteFile.SingleOrDefault(p => p.NoteFileName == inputModel.linkedfile);
            if (file == null)
                return "Target file does not exist";

            // check for acceptance

            if (!await AccessManager.TestLinkAccess(_context, file, inputModel.Secret))
                return "Access Denied";

            // find local base note for this and modify header

            NoteHeader extant = _context.NoteHeader.SingleOrDefault(p => p.LinkGuid == inputModel.baseGuid);

            if (extant == null) // || extant.NoteFileId != file.Id)
                return "Could not find base note";

            inputModel.header.NoteOrdinal = extant.NoteOrdinal;

            inputModel.header.NoteFileId = file.Id;

            inputModel.header.BaseNoteId = extant.BaseNoteId;
            inputModel.header.Id = 0;
            inputModel.header.NoteContent = null;
            inputModel.header.NoteFile = null;
            //inputModel.header.ResponseOrdinal = 0;
            //inputModel.header.ResponseCount = 0;

            var tags = Tags.ListToString(inputModel.tags);

            NoteHeader nh = await NoteDataManager.CreateResponse(_context, null, inputModel.header,
                inputModel.content.NoteBody, tags, inputModel.content.DirectorMessage, true, true);

            if (nh == null)
            {

                return "Remote response create failed";
            }

            return "Ok";
        }

        [HttpGet]
        public async Task<string> Get(string file)
        {
            NoteFile nf = _context.NoteFile.SingleOrDefault(p => p.NoteFileName == file);

            if (nf != null)
                return "Ok";

            return "File '" + file + "' does not exist on remote system.";
        }

    }
}
