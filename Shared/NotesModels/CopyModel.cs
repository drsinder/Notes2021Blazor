using System;
using System.Collections.Generic;
using System.Text;

namespace Notes2021Blazor.Shared
{
    public class CopyModel
    {
        public NoteHeader Note { get; set; }
        public int FileId { get; set; }
        public bool WholeString { get; set; }
        public UserData UserData { get; set; }
    }
}
