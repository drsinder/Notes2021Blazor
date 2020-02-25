using System;
using System.Collections.Generic;
using System.Text;

namespace Notes2021Blazor.Shared
{
    public enum Commands
    {
        CloseMe,
        Show,
        Dialog,
        Refresh,
        CloseMeAndRefresh,
        Home
    }

    public enum Locations
    {
        Home,
        Login,
        Logout,
        Register,
        FileUserIndex,
        MainHelp,
        About,
        Schema
    }

    public class Message
    {
        public Commands Command { get; set; }
        public Locations Location { get; set; }
        public int IntArg { get; set; }
        public string StringArg { get; set; }

    }
}
