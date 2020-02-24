using System;
using System.Collections.Generic;
using System.Text;

namespace Notes2021Blazor.Shared
{
    public class JsonExport
    {
        public NoteFile NoteFile { get; set; }
        public List<NoteHeader> NoteHeaders { get; set; }
        public List<NoteContent> NoteContents { get; set; }
        public List<Tags> Tags { get; set; }
    }
}
