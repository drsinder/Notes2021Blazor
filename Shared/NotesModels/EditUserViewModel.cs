using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;


namespace Notes2021Blazor.Shared
{
    public class CheckedUser
    {
        public IdentityRole theRole { get; set; }

        public bool isMember { get; set; }
    }

    public class EditUserViewModel
    {
        public UserData UserData { get; set; }
        public List<CheckedUser> RolesList { get; set; }
    }

}
