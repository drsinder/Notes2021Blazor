/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: NoteHeader.cs
    **
    ** Description:
    **      Header for a note - every note, base or response has one
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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notes2021Blazor.Shared
{
    public class NoteHeader
    {
        // Uniquely identifies the note
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        // The fileid the note belongs to
        [Required]
        public int NoteFileId { get; set; }

        [ForeignKey("NoteFileId")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public NoteFile NoteFile { get; set; }

        [Required]
        public int ArchiveId { get; set; }

        public long BaseNoteId { get; set; }

        // the ordinal on a Base note and all its responses
        [Required]
        [Display(Name = "Note #")]
        public int NoteOrdinal { get; set; }

        // The ordinal of the response where 0 is a Base Note
        [Required]
        [Display(Name = "Response #")]
        public int ResponseOrdinal { get; set; }

        // Subject/Title of a note
        [Required]
        [StringLength(200)]
        [Display(Name = "Subject")]
        public string NoteSubject { get; set; }

        // When the note was created or last edited
        [Required]
        [Display(Name = "Last Edited")]
        public DateTime LastEdited { get; set; }

        // When the thread was last edited
        [Required]
        [Display(Name = "Thread Last Edited")]
        public DateTime ThreadLastEdited { get; set; }

        [Required]
        [Display(Name = "Created")]
        public DateTime CreateDate { get; set; }

        public DateTime CreateDateLocal { get 
            {
                int OHours = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours;
                int OMinutes = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Minutes;

                return CreateDate.AddHours(OHours).AddMinutes(OMinutes);
            }
        }

        // Meaningful only if ResponseOrdinal = 0
        [Required]
        public int ResponseCount { get; set; }

        // ReSharper disable once InconsistentNaming
        [StringLength(450)]
        public string AuthorID { get; set; }

        [Required]
        [StringLength(50)]
        public string AuthorName { get; set; }

        [StringLength(100)]
        public string LinkGuid { get; set; }

        public NoteContent NoteContent { get; set; }

        public List<Tags> Tags { get; set; }

        public NoteHeader CloneForLink()
        {
            NoteHeader nh = new NoteHeader()
            {
                Id = Id,
                NoteSubject = NoteSubject,
                LastEdited = LastEdited,
                ThreadLastEdited = ThreadLastEdited,
                CreateDate = CreateDate,
                AuthorID = AuthorID,
                AuthorName = AuthorName,
                LinkGuid = LinkGuid
            };

            return nh;
        }

        public NoteHeader CloneForLinkR()
        {
            NoteHeader nh = new NoteHeader()
            {
                Id = Id,
                NoteSubject = NoteSubject,
                LastEdited = LastEdited,
                ThreadLastEdited = ThreadLastEdited,
                CreateDate = CreateDate,
                AuthorID = AuthorID,
                AuthorName = AuthorName,
                LinkGuid = LinkGuid,
                ResponseOrdinal = ResponseOrdinal
            };

            return nh;
        }
    }
}
