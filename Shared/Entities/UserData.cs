﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Notes2021Blazor.Shared
{
    public class UserData
    {
        [Required]
        [Key]
        [StringLength(450)]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Display Name")]
        [StringLength(50)]
        [PersonalData]
        public string DisplayName { get; set; }

        [PersonalData]
        public int TimeZoneID { get; set; }

        public int Ipref2 { get; set; } // user choosen page size

        public int Ipref3 { get; set; }

        public int Ipref4 { get; set; }

        public int Ipref5 { get; set; }

        public int Ipref6 { get; set; }

        public int Ipref7 { get; set; }

        public int Ipref8 { get; set; }


        [PersonalData]
        public bool Pref1 { get; set; } // false = use paged note index, true= scrolled

        [PersonalData]
        public bool Pref2 { get; set; } // use alternate editor

        public bool Pref3 { get; set; } // show responses by default

        public bool Pref4 { get; set; }

        public bool Pref5 { get; set; }

        public bool Pref6 { get; set; }

        public bool Pref7 { get; set; }

        public bool Pref8 { get; set; }

        [Display(Name = "Style Preferences")]
        [StringLength(7000)]
        [PersonalData]
        public string MyStyle { get; set; }

        [StringLength(100)]
        public string MyGuid { get; set; }

    }
}
