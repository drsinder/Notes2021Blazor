/*--------------------------------------------------------------------------
**
**  Copyright (c) 2019, Dale Sinder
**
**  Name: HomePageModel.cs
**
**  Description:
**      Used for the root "/" of the app
**
**  This program is free software: you can redistribute it and/or modify
**  it under the terms of the GNU General Public License version 3 as
**  published by the Free Software Foundation.
**  
**  This program is distributed in the hope that it will be useful,
**  but WITHOUT ANY WARRANTY; without even the implied warranty of
**  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
**  GNU General Public License version 3 for more details.
**  
**  You should have received a copy of the GNU General Public License
**  version 3 along with this program in file "license-gpl-3.0.txt".
**  If not, see <http://www.gnu.org/licenses/gpl-3.0.txt>.
**
**--------------------------------------------------------------------------
*/

using System.Collections.Generic;

namespace Notes2021Blazor.Shared
{
    public class HomePageModel
    {
        public List<NoteFile> NoteFiles { get; set; }

        public List<NoteAccess> NoteAccesses { get; set; }

        public TZone TimeZone { get; set; }

        public HomePageMessage Message { get; set; }

        public UserData UserData { get; set; }
    }
}
