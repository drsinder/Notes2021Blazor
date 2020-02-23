/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: UserData.cs
    **
    ** Description:
    **      User Preferences
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

        public int Ipref8 { get; set; } // bits extend bool properties


        [PersonalData]
        public bool Pref1 { get; set; } // false = use paged note index, true= scrolled

        [PersonalData]
        public bool Pref2 { get; set; } // use alternate editor

        public bool Pref3 { get; set; } // show responses by default

        public bool Pref4 { get; set; } // multiple expanded responses

        public bool Pref5 { get; set; } // expanded responses

        public bool Pref6 { get; set; } // alternate text editor

        public bool Pref7 { get; set; } // show content when expanded

        public bool Pref8 { get; set; }

        //public bool Pref9
        //{
        //    get { return (Ipref8 & 1) == 1; }
        //    set
        //    {
        //        Ipref8 = Ipref8 & 0x7ffffffe;
        //        if (value)
        //            Ipref8 += 1;
        //    }
        //}

        //public bool Pref10
        //{
        //    get { return (Ipref8 & 2) == 2; }
        //    set
        //    {
        //        Ipref8 = Ipref8 & 0x7ffffffd;
        //        if (value)
        //            Ipref8 += 2;
        //    }
        //}

        //public bool Pref11
        //{
        //    get { return (Ipref8 & 4) == 4; }
        //    set
        //    {
        //        Ipref8 = Ipref8 & 0x7ffffffB;
        //        if (value)
        //            Ipref8 += 4;
        //    }
        //}

        //public bool Pref12
        //{
        //    get { return (Ipref8 & 8) == 8; }
        //    set
        //    {
        //        Ipref8 = Ipref8 & 0x7ffffff7;
        //        if (value)
        //            Ipref8 += 8;
        //    }
        //}

        //public bool Pref13
        //{
        //    get { return (Ipref8 & 16) == 16; }
        //    set
        //    {
        //        Ipref8 = Ipref8 & 0x7fffffef;
        //        if (value)
        //            Ipref8 += 16;
        //    }
        //}

        //public bool Pref14
        //{
        //    get { return (Ipref8 & 32) == 32; }
        //    set
        //    {
        //        Ipref8 = Ipref8 & 0x7fffffdf;
        //        if (value)
        //            Ipref8 += 32;
        //    }
        //}

        //public bool Pref15
        //{
        //    get { return (Ipref8 & 64) == 64; }
        //    set
        //    {
        //        Ipref8 = Ipref8 & 0x7fffffBf;
        //        if (value)
        //            Ipref8 += 64;
        //    }
        //}

        //public bool Pref16
        //{
        //    get { return (Ipref8 & 128) == 128; }
        //    set
        //    {
        //        Ipref8 = Ipref8 & 0x7fffff7f;
        //        if (value)
        //            Ipref8 += 128;
        //    }
        //}

        [Display(Name = "Style Preferences")]
        [StringLength(7000)]
        [PersonalData]
        public string MyStyle { get; set; }

        [StringLength(100)]
        public string MyGuid { get; set; }

    }
}
