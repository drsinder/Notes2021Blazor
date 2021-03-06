﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Notes2021Blazor.Shared;
using Syncfusion.EJ2.Blazor.Navigations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Web;
using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Newtonsoft.Json;
using System.Timers;
using System.ComponentModel.DataAnnotations;
using Notes2021Spa.RCL.Pages.Notes;

namespace Notes2021Spa.RCL.Pages.Root
{

    public class IndexComp : ComponentBase
    {
        [Inject]
        HttpClient Http { get; set; }
        [Inject]
        ILocalStorageService _localStorage { get; set; }
        [Inject]
        IModalService Modal { get; set; }
        [Inject]
        NavigationManager navManager { get; set; }

        [Parameter] public EventCallback<Message> OnMessage { get; set; }

        protected Locations CurrentLoc { get; set; }
        protected Locations PreviousLoc { get; set; }

        protected bool ShowIndex { get; set; }

        protected string Autologin { get; set; }

        protected List<object> menuItemsTop { get; set; }

        protected int IntArg { get; set; }
        protected string StringArg { get; set; }

        protected Task<AuthenticationState> authenticationStateTask { get; set; }

        public string fileId { get; set; }

        //public string Routing { get; set; }

        protected bool newnoteFlag { get; set; }
        public long headerId { get; set; }
        protected int Id { get; set; }
        protected int archiveID { get; set; }
        protected NoteDisplayIndexModel Model { get; set; }
        protected long mode { get; set; }
        protected int basenoteCount { get; set; }
        protected string message = "Loading...";
        protected LocalInput myInput { get; set; }
        protected NoteHeader currentHeader { get; set; }

        protected string scroller { get; set; }

        //protected Timer timer { get; set; }
        //protected Timer timer2 { get; set; }
        protected Timer timerAccess1Trigger { get; set; }
        protected Timer timerAccess2Trigger { get; set; }

        protected List<Sequencer> trackers { get; set; }
        protected int seqIndex { get; set; }

        protected List<NoteHeader> seqBases { get; set; }

        protected int mailOrd { get; set; }

        protected bool isIndexSearch { get; set; }

        protected List<NoteHeader> results { get; set; }

        protected bool isSearch { get; set; }

        protected bool printfile { get; set; }

        protected EjsMenu topMenu { get; set; }

        #region SPA Stuff
        protected override async Task OnInitializedAsync()
        {
            ShowIndex = true;

            await MakeMenu();

            // check for parameters to show a note or a filename
            var uri = navManager.ToAbsoluteUri(navManager.Uri); navManager.ToAbsoluteUri(navManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("show", out var param))
            {
                long NoteId = long.Parse(param.First());
                await EnterAndShow(NoteId);
                PreviousLoc = Locations.Home;
                CurrentLoc = Locations.FileUserIndex;
                StateHasChanged();
                return;
            }
            else if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("filename", out var filename))
            {
                string myfile = filename.First();

                HomePageModel files = await Http.GetJsonAsync<HomePageModel>("api/HomePageData");

                NoteFile x = files.NoteFiles.Find(p => p.NoteFileName == myfile);
                if (x == null)
                    return;

                fileId = x.Id.ToString();
                archiveID = 0;
                await MainEntry();
                PreviousLoc = Locations.Home;
                CurrentLoc = Locations.FileUserIndex;
                return;
            }
            else
                PreviousLoc = CurrentLoc = Locations.Home;
        }

        /// <summary>
        /// Create the elements of the always present menu
        /// </summary>
        /// <returns></returns>
        protected async Task MakeMenu()
        {
            menuItemsTop = new List<object>();
            object item;

            item = new { id = "Recent", text = "Recent Notes" };
            menuItemsTop.Add(item);

            item = new { id = "Manage", text = "Manage" };
            menuItemsTop.Add(item);
            item = new { id = "MRecent", text = "Recent", parentId = "Manage" };
            menuItemsTop.Add(item);
            item = new { id = "Subscriptions", text = "Subscriptions", parentId = "Manage" };
            menuItemsTop.Add(item);
            item = new { id = "Preferences", text = "Preferences", parentId = "Manage" };
            menuItemsTop.Add(item);

            item = new { id = "Help", text = "Help" };
            menuItemsTop.Add(item);
            item = new { id = "MainHelp", text = "Help", parentId = "Help" };
            menuItemsTop.Add(item);
            item = new { id = "About", text = "About", parentId = "Help" };
            menuItemsTop.Add(item);
            item = new { id = "License", text = "License", parentId = "Help" };
            menuItemsTop.Add(item);

            item = new { id = "Admin", text = "Admin" };
            menuItemsTop.Add(item);
            item = new { id = "NoteFiles", text = "NoteFiles", parentId = "Admin" };
            menuItemsTop.Add(item);
            item = new { id = "Roles", text = "User Roles", parentId = "Admin" };
            menuItemsTop.Add(item);
            item = new { id = "Linked", text = "Linked Files", parentId = "Admin" };
            menuItemsTop.Add(item);

            await UpdateMenu();
        }

        /// <summary>
        /// Enable only items available to logged in user
        /// </summary>
        /// <returns></returns>
        protected async Task UpdateMenu()
        {
            bool isAdmin = false;
            bool isUser = false;
            try
            {
                UserData udata = await Http.GetJsonAsync<UserData>("api/User");
                string uid = udata.UserId;
                EditUserViewModel Umodel = await Http.GetJsonAsync<EditUserViewModel>("api/UserEdit/" + uid);

                foreach (CheckedUser u in Umodel.RolesList)
                {
                    if (u.theRole.NormalizedName == "ADMIN" && u.isMember)
                    {
                        isUser = isAdmin = true;
                    }
                    if (u.theRole.NormalizedName == "USER" && u.isMember)
                    {
                        isUser = true;
                    }
                }
            }
            catch (Exception e)
            {
                //ShowMessage(e.Message);
            }
            await topMenu.EnableItems(new string[] { "Recent Notes", "Manage", "Admin" }, false);
            if (isUser || isAdmin)
            {
                await topMenu.EnableItems(new string[] { "Recent Notes", "Manage" }, true);
            }
            if (isAdmin)
            {
                await topMenu.EnableItems(new string[] { "Admin" }, true);
            }
        }

        /// <summary>
        /// Handle always present menu item selections.
        /// Convert menu item Ids to actions (sub-pages)
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task OnSelectTop(MenuEventArgs e)  // top level menu event handler
        {
            PreviousLoc = CurrentLoc;

            switch (e.Item.Id)
            {
                case "MainHelp":
                    CurrentLoc = Locations.MainHelp; break;
                case "About":
                    CurrentLoc = Locations.About; break;
                case "Preferences":
                    CurrentLoc = Locations.Preferences; break;
                case "MRecent":
                    CurrentLoc = Locations.RecentEdit; break;
                case "Subscriptions":
                    CurrentLoc = Locations.Subscriptions; break;
                case "License":
                    CurrentLoc = Locations.License; break;
                case "NoteFiles":
                    CurrentLoc = Locations.AdminFiles; break;
                case "Roles":
                    CurrentLoc = Locations.Roles; break;
                case "Linked":
                    CurrentLoc = Locations.Linked; break;
                case "Recent":
                    CurrentLoc = Locations.FileUserIndex;
                    await StartListing("sequence");
                    break;

                default: break;
            }
        }

        /// <summary>
        /// Index (SPA) message handler - show different sub-pages
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public async Task MessageHandler(Message Message)
        {
            await UpdateMenu();

            switch (Message.Command)
            {
                case Commands.CloseMe:
                    CurrentLoc = PreviousLoc;
                    PreviousLoc = Locations.Home;
                    break;
                case Commands.CloseMeAndRefresh:
                    await UpdateMenu();
                    CurrentLoc = PreviousLoc;
                    PreviousLoc = Locations.Home;
                    StateHasChanged();
                    break;
                case Commands.Show:
                    IntArg = Message.IntArg;
                    StringArg = Message.StringArg;
                    PreviousLoc = CurrentLoc;
                    CurrentLoc = Message.Location;
                    if (Message.Location == Locations.FileUserIndex)
                    {
                        fileId = IntArg.ToString();
                        await StartListing("main");
                    }
                    break;
                case Commands.Home:
                    PreviousLoc = Locations.Home;
                    CurrentLoc = PreviousLoc;
                    break;
                case Commands.Dialog:
                    if (Message.Location == Locations.Login)
                    {
                        Autologin = Message.StringArg;

                        var parameters = new ModalParameters();
                        parameters.Add("Autologin", Autologin);
                        Modal.OnClose += HideDialogLogin;
                        Modal.Show<Login>("", parameters);
                    }
                    break;
                case Commands.Refresh:
                    await UpdateMenu();
                    StateHasChanged();
                    break;
                default:
                    break;
            }
        }

        protected void Login()
        {
            Autologin = "x";
            var parameters = new ModalParameters();
            parameters.Add("Autologin", Autologin);
            Modal.OnClose += HideDialogLogin;
            Modal.Show<Login>("", parameters);
        }

        protected void HideDialogLogin(ModalResult modalResult)
        {
            Modal.OnClose -= HideDialogLogin;
            CurrentLoc = Locations.Temp;
            StateHasChanged();
        }

        protected void Register()
        {
            PreviousLoc = CurrentLoc;
            CurrentLoc = Locations.Register;
        }

        protected void Logout()
        {
            PreviousLoc = Locations.Home;
            CurrentLoc = Locations.Logout;
        }

        protected void Home()
        {
            UpdateMenu().GetAwaiter();
            PreviousLoc = Locations.Home;
            CurrentLoc = Locations.Home;
        }

        #endregion

        // 
        // Above here related to SPA Index (menu, routing)
        // 
        // Below here related to the Note Listing for a file
        // 

        #region Notes List And Display
        /// <summary>
        /// Notes Listing message handler.  Other messages from other pages related to display of index
        /// and notes content bubble to here.
        /// </summary>
        /// <param name="newMessage">Indicates what should happen</param>
        /// <returns></returns>
        protected async Task ClickHandler(string newMessage)
        {
            if (newMessage.StartsWith("Done:"))
            {
                mode = 0;
                this.StateHasChanged();
                int nord = int.Parse(newMessage.Substring(5, newMessage.Length - 5));

                if (nord > 1)
                    nord--;
                scroller = "Base" + nord;
            }
            else if (newMessage.StartsWith("Note:"))
            {
                string notenum = newMessage.Substring(5, newMessage.Length - 5);
                long newmode = long.Parse(notenum);

                currentHeader = Model.AllNotes.Find(p => p.Id == newmode);

                mode = newmode;
                this.StateHasChanged();
            }
            else if (newMessage.StartsWith("Html:"))
            {
                int nord = int.Parse(newMessage.Substring(5, newMessage.Length - 5));
                DoExport1(nord, true, true);
            }
            else if (newMessage.StartsWith("html:"))
            {
                int nord = int.Parse(newMessage.Substring(5, newMessage.Length - 5));
                DoExport1(nord, true, false);
            }
            else if (newMessage.StartsWith("mail:"))
            {
                mailOrd = int.Parse(newMessage.Substring(5, newMessage.Length - 5));
                mail1();
            }
            else if (newMessage.StartsWith("EnterAndShow:"))
            {
                long showme = long.Parse(newMessage.Substring(13, newMessage.Length - 13));
                await EnterAndShow(showme);
            }
            else
            {
                switch (newMessage)
                {
                    case "ListHelp": ShowHelp(); break;
                    case "NewBaseNote":
                        NewBaseNote();
                        break;
                    case "CancelEdit":
                        Done();
                        break;
                    case "AccessControls": EditAccess(); break;
                    case "ListNoteFiles": ListFiles(); break;
                    case "SeqFileDone":
                        var stringContent = new StringContent(JsonConvert.SerializeObject(trackers[seqIndex]), Encoding.UTF8, "application/json");
                        HttpResponseMessage result = await Http.PutAsync("api/Sequencer/", stringContent);
                        //NavigationManager.NavigateTo("notes/sequence", true);
                        break;
                    case "HtmlFromIndex": HtmlFromIndex(); break;
                    case "htmlFromIndex": htmlFromIndex(); break;
                    case "eXport": eXport(); break;
                    case "mailFromIndex": mail(); break;
                    case "mail":
                        mail1(); break;
                    case "Search":
                        isIndexSearch = false;
                        Search();
                        break;
                    case "SearchFromIndex":
                        isIndexSearch = true;
                        Search();
                        break;
                    case "MarkMine":
                        await MarkMine();
                        break;
                    case "OutputMarked":
                        OutputMarkedAsk();
                        break;
                    case "PrintFile":
                        PrintFile();
                        break;
                    case "JsonExport":
                        DoJson();
                        break;
                    case "Done":
                        ShowIndex = false;
                        StateHasChanged();
                        await MainEntry();
                        StateHasChanged();
                        ShowIndex = true;
                        Done();
                        break;

                    default:
                        ShowMessage(newMessage + " Clicked");
                        break;
                }

            }
        }

        /// <summary>
        /// Does the appropriate startup thing based on the value of Routing
        /// </summary>
        /// <param name="Routing">Indicates what to do</param>
        /// <returns></returns>
        protected async Task StartListing(string Routing)
        {
            scroller = string.Empty;
            seqIndex = -1;
            //if (Routing == ("display"))  // display a base note from the main index
            //{
            //    headerId = long.Parse(fileId);
            //    currentHeader = Model.Notes.Find(p => p.Id == headerId);

            //    mode = currentHeader.Id;
            //    this.StateHasChanged();
            //}
            if (Routing == "main")     // show the main index
            {
                archiveID = 0;
                await MainEntry();
            }
            //if (Routing == ("enterandshow")) // load the index data and then display a note
            //{
            //    await EnterAndShow(0);
            //}
            if (Routing == ("sequence"))
            {
                trackers = await Http.GetJsonAsync<List<Sequencer>>("api/sequencer");
                if (trackers == null || trackers.Count < 1)
                {
                    message = null;
                    //await OnMessage.InvokeAsync(new Message { Command = Commands.Home });

                    ShowMessage("Completed!");
                    Home();
                    return;
                }
                await StartFileSequence();
            }
        }

        /// <summary>
        /// Load file headers and show a note
        /// </summary>
        /// <param name="it">Id of note to show</param>
        /// <returns></returns>
        protected async Task EnterAndShow(long it)
        {
            // fileId is really noteId so use it to get base note header

            long myNoteId = it;
            if (myNoteId == 0)
                myNoteId = long.Parse(fileId);

            NoteHeader myHeader = await Http.GetJsonAsync<NoteHeader>("api/EnterAndDisplay/" + myNoteId);
            Id = myHeader.NoteFileId;  // we now know the file.  So load it up

            archiveID = await _localStorage.GetItemAsync<int>("ArchiveId");

            myInput = new LocalInput();
            myInput.isAutoFocus = true;

            try
            {
                Model = await Http.GetJsonAsync<NoteDisplayIndexModel>("api/NoteIndex/" + Id + "." + archiveID);
                Model.Notes = Model.AllNotes.Where(p => p.ResponseOrdinal == 0).OrderBy(p => p.NoteOrdinal).ToList();
            }
            catch (Exception ex)
            {
                Model = new NoteDisplayIndexModel { message = ex.Message };
                message = ex.Message;
                return;
            }

            if (Model == null)
            {
                message = "Model is null";
                return;
            }

            if (!string.IsNullOrEmpty(Model.message))
                message = Model.message;

            basenoteCount = Model.Notes.Count;

            currentHeader = myHeader;

            message = null;
            newnoteFlag = false;
            mode = myNoteId;
            StateHasChanged();
        }

        /// <summary>
        /// Load file headers and show listing of notes
        /// </summary>
        /// <returns></returns>
        protected async Task MainEntry()
        {
            Id = int.Parse(fileId);
            //archiveID = await _localStorage.GetItemAsync<int>("ArchiveId");

            mode = 0;  // index mode

            myInput = new LocalInput();
            myInput.isAutoFocus = true;

            try
            {
                Model = await Http.GetJsonAsync<NoteDisplayIndexModel>("api/NoteIndex/" + Id + "." + archiveID);
                Model.Notes = Model.AllNotes.Where(p => p.ResponseOrdinal == 0).OrderBy(p => p.NoteOrdinal).ToList();
            }
            catch (Exception ex)
            {
                Model = new NoteDisplayIndexModel { message = ex.Message };
            }

            if (Model == null)
            {
                message = "Model is null";
                return;
            }

            if (Model.UserData == null)
            {
                message = "UserData is null";
                return;
            }

            if (!string.IsNullOrEmpty(Model.message))
                message = Model.message;

            if (Model.Notes.Count > 0)
                currentHeader = Model.Notes[0];
            else
                currentHeader = new NoteHeader { NoteOrdinal = 1 };

            message = null;

            basenoteCount = Model.Notes.Count;

            StateHasChanged();
        }

        /// <summary>
        /// Set a timer to focus type in box
        /// </summary>
        /// <param name="firstRender"></param>
        protected override void OnAfterRender(bool firstRender)
        {
            if (CurrentLoc == Locations.FileUserIndex)
            {
                if (firstRender)
                {
                    //timer = new Timer(1000);        // we can not put focus in the nav box here so defer it
                    //timer.Elapsed += TimerTick;
                    //timer.Enabled = true;

                    //timer2 = new Timer(1000);
                    //timer2.Elapsed += TimerTick2;
                    //timer2.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Clear print flag in rare case it is set and focus the type in box
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        //protected void TimerTick(Object source, ElapsedEventArgs e)
        //{
        //    printfile = false;
        //    //timer.Interval = 5000;
        //    timer.Stop();
        //    timer.Enabled = false;
        //    jsRuntime.InvokeAsync<object>("setfocus", "arrow1");
        //}

        //protected void TimerTick2(Object source, ElapsedEventArgs e)
        //{
        //    if (!string.IsNullOrEmpty(scroller))
        //    {
        //        jsRuntime.InvokeAsync<object>("setlocation", scroller);
        //        scroller = string.Empty;
        //    }
        //}

        protected void StartTimer()
        {
            //timer = new Timer(1000);
            //timer.Elapsed += TimerTick;
            //timer.Enabled = true;
        }

        protected void StopTimer()
        {
            //timer.Enabled = false;
            //timer.Stop();
        }

        /// <summary>
        /// Start looking the new notes since last the user did this
        /// </summary>
        /// <returns></returns>
        protected async Task StartFileSequence()
        {
            seqBases = null;
            while ((++seqIndex < trackers.Count) && (seqBases == null || seqBases.Count == 0))
            {
                NoteFile nf = await PreLoadSeqFile();
                if (nf.LastEdited < trackers[seqIndex].LastTime)  // quick check when file last changed
                    continue;       // nothing new here

                await LoadSeqFile();
                seqBases = SearchForTrackedBaseNotes(trackers[seqIndex]);
                if (seqBases != null && seqBases.Count > 0)
                    break;
            }
            if ((seqIndex >= trackers.Count) || (seqBases == null || seqBases.Count == 0))
            {
                var parameters = new ModalParameters();
                Modal.OnClose += HideDialog;
                mode = -1;
                Modal.Show<Completed>("", parameters);

                Home();

                return;
            }
            message = null;
            currentHeader = seqBases[0];
            mode = currentHeader.Id;
            StateHasChanged();
        }

        /// <summary>
        /// Loads the NoetFile object for a file to be sequenced
        /// </summary>
        /// <returns></returns>
        protected async Task<NoteFile> PreLoadSeqFile()
        {
            Sequencer tr = trackers[seqIndex];
            Id = tr.NoteFileId;

            mode = 0;  // index mode

            return await Http.GetJsonAsync<NoteFile>("api/NoteIndexFile/" + Id);
        }

        /// <summary>
        /// load the file data to search during sequence
        /// </summary>
        /// <returns></returns>
        protected async Task LoadSeqFile()
        {
            Sequencer tr = trackers[seqIndex];
            Id = tr.NoteFileId;

            archiveID = 0;

            mode = 0;  // index mode

            myInput = new LocalInput();
            myInput.isAutoFocus = true;

            try
            {
                Model = await Http.GetJsonAsync<NoteDisplayIndexModel>("api/NoteIndex/" + Id + "." + archiveID);
                Model.Notes = Model.AllNotes.Where(p => p.ResponseOrdinal == 0).OrderBy(p => p.NoteOrdinal).ToList();
            }
            catch (Exception ex)
            {
                Model = new NoteDisplayIndexModel { message = ex.Message };
            }

            if (Model == null)
            {
                message = "Model is null";
                return;
            }

            if (!string.IsNullOrEmpty(Model.message))
                message = Model.message;

            message = null;

            basenoteCount = Model.Notes.Count;
        }

        /// <summary>
        /// Do the check for a note written since last visit
        /// </summary>
        /// <param name="tr">Sequencer/tracker item we are working on</param>
        /// <returns></returns>
        protected List<NoteHeader> SearchForTrackedBaseNotes(Sequencer tr)
        {
            List<NoteHeader> bnh = Model.Notes.Where(p => p.ThreadLastEdited > tr.LastTime).OrderBy(p => p.NoteOrdinal).ToList();
            return bnh;
        }

        /// <summary>
        /// Show list of files
        /// </summary>
        protected void ListFiles()
        {
            PreviousLoc = CurrentLoc;
            CurrentLoc = Locations.FileList;
        }

        /// <summary>
        /// Handle a keyboard key press
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        protected async Task KeyPressed(KeyboardEventArgs eventArgs)
        {
            switch (eventArgs.Key)
            {
                case "Z": ShowHelp(); break;
                case "N":
                    if (Model.myAccess.Write)
                    {
                        NewBaseNote();
                    }
                    break;
                case "A":
                    if (Model.myAccess.EditAccess || Model.myAccess.ViewAccess)
                    {
                        EditAccess();
                    }
                    break;
                case "a":
                    ShowArchive();
                    break;
                case "L":
                    ListFiles();
                    break;
                case "H":
                    HtmlFromIndex();
                    break;
                case "h":
                    htmlFromIndex();
                    break;
                case "X":
                    eXport();
                    break;
                case "m":
                    mail();
                    break;
                case "S":
                    isIndexSearch = true;
                    Search();
                    break;
                case "M":
                    await MarkMine();
                    break;
                case "O":
                    if (Model.isMarked)
                        OutputMarkedAsk();
                    break;
                case "P":
                    PrintFile();
                    break;
                case "R":
                    await MainEntry();
                    break;
                case "U":
                    Browse();
                    break;
                case "J":
                    DoJson();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Start a new Note
        /// </summary>
        void NewBaseNote()
        {
            StopTimer();
            newnoteFlag = true;
            this.StateHasChanged();
        }

        /// <summary>
        /// Handle typed input for navigation
        /// </summary>
        /// <param name="typedInput"></param>
        protected void TextHasChanged(string typedInput)
        {
            typedInput = typedInput.Trim().Replace("'\n", "").Replace("'\r", "").Trim();

            int fileId = Model.noteFile.Id;
            int noteOrd = 1;
            if (string.IsNullOrEmpty(typedInput) || string.IsNullOrWhiteSpace(typedInput))
                return;

            if (typedInput.Contains("."))
            {
                string[] splits = typedInput.Split(new[] { '.' });
                if (splits.Length != 2)
                {
                    return;
                }
                bool ax = !int.TryParse(splits[0], out noteOrd);
                bool bx = !int.TryParse(splits[1], out var respOrd);
                if (ax || bx)
                {
                    return;
                }
                currentHeader = Model.AllNotes.Where(p => p.NoteOrdinal == noteOrd && p.ResponseOrdinal == respOrd).SingleOrDefault();
                if (currentHeader == null)
                    return;
                if (!Model.myAccess.ReadAccess)
                    return;
                mode = currentHeader.Id;
            }
            else
            {
                if (!int.TryParse(typedInput, out noteOrd))
                {
                    return;
                }
                if (noteOrd < 1 || noteOrd > Model.Notes.Count)
                {
                    return;
                }
                if (!Model.myAccess.ReadAccess)
                    return;

                currentHeader = Model.Notes[noteOrd - 1];
                mode = currentHeader.Id;
            }

            this.StateHasChanged();
        }

        /// <summary>
        /// Cancel whatever we were doing and show the note file listing
        /// </summary>
        protected void Done()
        {
            mode = 0;
            newnoteFlag = false;
            this.StateHasChanged();
        }

        /// <summary>
        /// Print the entire file
        /// </summary>
        protected void PrintFile()
        {
            StopTimer();
            currentHeader = Model.Notes[0];
            printfile = true;
            //timer = new Timer(3000);
            //timer.Elapsed += TimerTick;
            //timer.Enabled = true;
            StateHasChanged();
        }

        protected void ShowHelp()
        {
            var parameters = new ModalParameters();
            StopTimer();
            Modal.OnClose += HideDialog;
            Modal.Show<HelpDialog>("", parameters);
        }

        void HideDialog(ModalResult modalResult)
        {
            StartTimer();
            Modal.OnClose -= HideDialog;
        }

        /// <summary>
        /// Select archive dialog
        /// </summary>
        void ShowArchive()
        {
            if (Model.noteFile.NumberArchives == 0)
                return;
            var parameters = new ModalParameters();
            parameters.Add("NoteFile", Model.noteFile);
            Modal.OnClose += HideDialogArchive;
            StopTimer();
            Modal.Show<Archives>("", parameters);
        }

        void HideDialogArchive(ModalResult modalResult)
        {
            Modal.OnClose -= HideDialogArchive;
            StartTimer();
            if (modalResult.Cancelled)
                return;

            archiveID = (int)modalResult.Data;

            MainEntry().GetAwaiter();
            StateHasChanged();
        }

        protected void ShowMessage(string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("Message", message);
            Modal.OnClose += HideDialog;
            Modal.Show<MessageBox>("", parameters);
        }

        /// <summary>
        /// Access list editor (show dialog)
        /// </summary>
        protected void EditAccess()
        {
            var parameters = new ModalParameters();
            parameters.Add("FileId", Id);
            Modal.OnClose += HideDialogAccess;
            StopTimer();
            Modal.Show<AccessList>("", parameters);
        }

        protected List<UserData> udata;

        /// <summary>
        /// Cancel out of access editor or create a new entry
        /// </summary>
        /// <param name="modalResult"></param>
        protected void HideDialogAccess(ModalResult modalResult)
        {
            Modal.OnClose -= HideDialogAccess;

            if (modalResult.Cancelled)
            {
                StartTimer();
                return;
            }

            udata = (List<UserData>)modalResult.Data;

            timerAccess1Trigger = new Timer(250);
            timerAccess1Trigger.Enabled = true;
            timerAccess1Trigger.Elapsed += TriggerCreateAccess;

        }

        /// <summary>
        /// Show create a new user for access dialog
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void TriggerCreateAccess(Object source, ElapsedEventArgs e)
        {
            timerAccess1Trigger.Stop();
            timerAccess1Trigger.Enabled = false;

            var parameters = new ModalParameters();
            parameters.Add("NoteFile", Model.noteFile);
            parameters.Add("UserList", udata);
            Modal.OnClose += HideDialogNewAccess;
            Modal.Show<AddAccessDlg>("", parameters);
        }

        /// <summary>
        /// set up for return to access list editor
        /// </summary>
        /// <param name="modalResult"></param>
        void HideDialogNewAccess(ModalResult modalResult)
        {
            Modal.OnClose -= HideDialogNewAccess;

            timerAccess2Trigger = new Timer(250);
            timerAccess2Trigger.Enabled = true;
            timerAccess2Trigger.Elapsed += EditAccess0;
        }

        /// <summary>
        /// return to access list editor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void EditAccess0(Object source, ElapsedEventArgs e)
        {
            timerAccess2Trigger.Stop();
            timerAccess2Trigger.Enabled = false;
            EditAccess();
        }

        /// <summary>
        /// Make a Json representation of the file for output
        /// </summary>
        protected void DoJson()
        {
            var parameters = new ModalParameters();

            ExportViewModel vm = new ExportViewModel();
            vm.ArchiveNumber = archiveID;
            vm.NoteFile = Model.noteFile;

            parameters.Add("ExportViewModel", vm);

            Modal.OnClose += HideDialog;
            Modal.Show<ExportJson>("", parameters);
        }

        /// <summary>
        /// Export whole file as text, html, or email to someone
        /// </summary>
        /// <param name="isHtml">As Html if true, else text</param>
        /// <param name="isCollapsible">Fancy expandable/collapsible html</param>
        /// <param name="isEmail">Shoud email?</param>
        /// <param name="emailaddr">Email address</param>
        protected void DoExport(bool isHtml, bool isCollapsible, bool isEmail = false, string emailaddr = null)
        {
            var parameters = new ModalParameters();

            ExportViewModel vm = new ExportViewModel();
            vm.ArchiveNumber = archiveID;
            vm.isCollapsible = isCollapsible;
            vm.isDirectOutput = !isEmail;
            vm.isHtml = isHtml;
            vm.NoteFile = Model.noteFile;
            vm.NoteOrdinal = 0;
            vm.Email = emailaddr;

            parameters.Add("ExportViewModel", vm);
            parameters.Add("FileName", Model.noteFile.NoteFileName + (isHtml ? ".html" : ".txt"));

            Modal.OnClose += HideDialog;
            Modal.Show<ExportUtil1>("", parameters);
        }

        /// <summary>
        /// Export Note string as text, html, or email to someone
        /// </summary>
        /// <param name="NoteOrd">Note number to export</param>
        /// <param name="isHtml">As Html if true, else text</param>
        /// <param name="isCollapsible">Fancy expandable/collapsible html</param>
        /// <param name="isEmail">Shoud email?</param>
        /// <param name="emailaddr">Email address</param>
        /// <summary>
        protected void DoExport1(int NoteOrd, bool isHtml, bool isCollapsible, bool isEmail = false, string emailaddr = null)
        {
            var parameters = new ModalParameters();

            ExportViewModel vm = new ExportViewModel();
            vm.ArchiveNumber = archiveID;
            vm.isCollapsible = isCollapsible;
            vm.isDirectOutput = !isEmail;
            vm.isHtml = isHtml;
            vm.NoteFile = Model.noteFile;
            vm.NoteOrdinal = NoteOrd;
            vm.Email = emailaddr;

            parameters.Add("ExportViewModel", vm);
            parameters.Add("FileName", Model.noteFile.NoteFileName + (isHtml ? ".html" : ".txt"));

            Modal.OnClose += HideDialog;
            Modal.Show<ExportUtil1>("", parameters);
        }

        /// Several diffent simple setups for export follow...

        protected void HtmlFromIndex()
        {
            DoExport(true, true);
        }

        protected void htmlFromIndex()
        {
            DoExport(true, false);
        }

        protected void eXport()
        {
            DoExport(false, false);
        }

        protected void mail()
        {
            StopTimer();
            var parameters = new ModalParameters();
            Modal.OnClose += HideDialogMail;
            Modal.Show<Email>("", parameters);
        }

        void HideDialogMail(ModalResult modalResult)
        {
            Modal.OnClose -= HideDialogMail;
            StartTimer();
            if (!modalResult.Cancelled)
                DoExport(true, true, true, modalResult.Data.ToString());
        }

        protected void mail1()
        {
            StopTimer();
            var parameters = new ModalParameters();
            Modal.OnClose += HideDialogMail1;
            Modal.Show<Email>("", parameters);
        }

        void HideDialogMail1(ModalResult modalResult)
        {
            Modal.OnClose -= HideDialogMail1;
            StartTimer();
            if (!modalResult.Cancelled)
                DoExport1(mailOrd, true, true, true, modalResult.Data.ToString());
        }

        /// <summary>
        /// Dialog to set up a search
        /// </summary>
        void Search()
        {
            StopTimer();
            var parameters = new ModalParameters();
            parameters.Add("TimeZone", Model.tZone);

            //if (isIndexSearch)
            parameters.Add("Text", "Whole File");
            //else
            //    parameters.Add("Text", "Note String");

            Modal.OnClose += HideDialogSearch;
            Modal.Show<SearchDlg>("", parameters);
        }

        /// <summary>
        /// Start search based on specs collected
        /// </summary>
        /// <param name="modalResult"></param>
        void HideDialogSearch(ModalResult modalResult)
        {
            Modal.OnClose -= HideDialogSearch;
            StartTimer();
            if (modalResult.Cancelled)
                return;

            Search target = (Search)modalResult.Data;

            DoSearch(target).GetAwaiter();
        }

        /// <summary>
        /// Perform the search
        /// </summary>
        /// <param name="target">Search specs</param>
        /// <returns></returns>
        protected async Task DoSearch(Search target)
        {
            message = "Searching... Please Wait...";
            StateHasChanged();

            switch (target.Option)
            {
                case SearchOption.Author:
                case SearchOption.Title:
                case SearchOption.TimeIsAfter:
                case SearchOption.TimeIsBefore:
                    SearchHeader(target);
                    break;

                case SearchOption.Content:
                case SearchOption.DirMess:
                    await SearchContents(target);
                    break;

                case SearchOption.Tag:
                    await SearchTags(target);
                    break;
            }

            message = null;
            StateHasChanged();
        }

        /// <summary>
        /// Search header for info...
        /// </summary>
        /// <param name="target"></param>
        protected void SearchHeader(Search target)
        {
            results = new List<NoteHeader>();
            List<NoteHeader> lookin = Model.AllNotes;

            foreach (NoteHeader nh in lookin)
            {
                bool isMatch = false;
                switch (target.Option)
                {
                    case SearchOption.Author:
                        isMatch = nh.AuthorName.Contains(target.Text);
                        break;
                    case SearchOption.Title:
                        isMatch = nh.NoteSubject.ToLower().Contains(target.Text.ToLower());
                        break;
                    case SearchOption.TimeIsAfter:
                        isMatch = DateTime.Compare(nh.LastEdited, target.Time) > 0;
                        break;
                    case SearchOption.TimeIsBefore:
                        isMatch = DateTime.Compare(nh.LastEdited, target.Time) < 0;
                        break;
                }
                if (isMatch)
                    results.Add(nh);
            }

            if (results.Count == 0)
            {
                ShowMessage("Nothing Found.");
                return;
            }

            results = results.OrderBy(p => p.LastEdited).ToList();

            mode = results[0].Id;
            isSearch = true;
            StateHasChanged();
        }

        /// <summary>
        /// Search Content for info...
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected async Task SearchContents(Search target)
        {
            results = new List<NoteHeader>();
            List<NoteHeader> lookin = Model.AllNotes;

            foreach (NoteHeader nh in lookin)
            {
                DisplayModel dm = await Http.GetJsonAsync<DisplayModel>("api/NoteContent/" + nh.Id);
                NoteContent nc = dm.content;
                List<Tags> tags = dm.tags;

                bool isMatch = false;
                switch (target.Option)
                {
                    case SearchOption.Content:
                        isMatch = nc.NoteBody.ToLower().Contains(target.Text.ToLower());
                        break;
                    case SearchOption.DirMess:
                        isMatch = nc.DirectorMessage.ToLower().Contains(target.Text.ToLower());
                        break;
                }
                if (isMatch)
                    results.Add(nh);
            }

            if (results.Count == 0)
            {
                ShowMessage("Nothing Found.");
                return;
            }

            results = results.OrderBy(p => p.LastEdited).ToList();

            mode = results[0].Id;
            isSearch = true;
            StateHasChanged();
        }

        /// <summary>
        /// Search tags for a match...
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected async Task SearchTags(Search target)
        {
            List<Tags> tags = await Http.GetJsonAsync<List<Tags>>("api/Tags/" + Model.noteFile.Id);

            results = new List<NoteHeader>();

            foreach (Tags t in tags)
            {
                foreach (NoteHeader x in results)
                {
                    if (x.Id == t.NoteHeaderId)
                        break;
                    results.Add(Model.AllNotes.Find(p => p.Id == x.Id));
                }
            }

            if (results.Count == 0)
            {
                ShowMessage("Nothing Found.");
                return;
            }

            results = results.OrderBy(p => p.LastEdited).ToList();

            mode = results[0].Id;
            isSearch = true;
            StateHasChanged();
        }

        /// <summary>
        /// Mark notes involving this user for later output
        /// </summary>
        /// <returns></returns>
        protected async Task MarkMine()
        {
            List<Mark> list = await Http.GetJsonAsync<List<Mark>>("api/Mark");

            List<Mark> marks = new List<Mark>();
            int ord = 1;
            foreach (NoteHeader nh in Model.AllNotes)
            {
                if (nh.AuthorID == Model.myAccess.UserID)
                {
                    Mark myMark = new Mark();
                    myMark.UserId = Model.myAccess.UserID;
                    myMark.NoteFileId = nh.NoteFileId;
                    myMark.ArchiveId = nh.ArchiveId;
                    myMark.NoteHeaderId = nh.Id;
                    myMark.NoteOrdinal = nh.NoteOrdinal;
                    myMark.MarkOrdinal = ord++;
                    myMark.ResponseOrdinal = -1;

                    // Need to check if string already included in this notefile

                    List<Mark> list2 = marks.Where(p => p.NoteFileId == nh.NoteFileId
                        && p.ArchiveId == nh.ArchiveId
                        && p.NoteOrdinal == nh.NoteOrdinal)
                        .ToList();
                    if (list2 != null && list2.Count > 0)
                    {
                        continue;
                    }

                    // need to check if already in my marks

                    List<Mark> list3 = marks.Where(p => p.NoteFileId == nh.NoteFileId
                        && p.ArchiveId == nh.ArchiveId
                        && p.NoteOrdinal == nh.NoteOrdinal)
                        .ToList();
                    if (list3 != null && list3.Count > 0)
                    {
                        continue;
                    }

                    marks.Add(myMark);
                }
            }
            if (marks.Count > 0)
            {
                await Http.PutJsonAsync("api/Mark", marks);
                Model.isMarked = true;
                StateHasChanged();
            }

            ShowMessage("Mark Completed. Marked " + marks.Count + " Strings");

        }

        /// <summary>
        /// Dialog to ask format of output
        /// </summary>
        protected void OutputMarkedAsk()
        {
            StopTimer();
            var parameters = new ModalParameters();
            Modal.OnClose += HideDialogOutputAsk;
            Modal.Show<OutputDlg>("", parameters);
        }

        /// <summary>
        /// Start output of marked notes
        /// </summary>
        /// <param name="modalResult"></param>
        protected void HideDialogOutputAsk(ModalResult modalResult)
        {
            Modal.OnClose -= HideDialogOutputAsk;
            StartTimer();
            if (!modalResult.Cancelled)
            {
                MarkedOutputModel mo = (MarkedOutputModel)modalResult.Data;
                OutputMarked(mo.isHtml, mo.isCollapsible, mo.isEmail, mo.Email);
            }
        }

        /// <summary>
        /// Do toe marked note output
        /// </summary>
        /// <param name="isHtml"></param>
        /// <param name="isCollapsible"></param>
        /// <param name="isEmail"></param>
        /// <param name="Email"></param>
        /// <returns></returns>
        protected async Task OutputMarked(bool isHtml, bool isCollapsible, bool isEmail, string Email)
        {
            List<Mark> marks = await Http.GetJsonAsync<List<Mark>>("api/Mark");

            var parameters = new ModalParameters();

            ExportViewModel vm = new ExportViewModel();
            vm.ArchiveNumber = archiveID;
            vm.isCollapsible = isCollapsible;
            vm.isDirectOutput = !isEmail;
            vm.isHtml = isHtml;
            vm.NoteFile = Model.noteFile;
            vm.NoteOrdinal = 0;
            vm.Email = Email;
            vm.Marks = marks.Where(p => p.NoteFileId == Model.noteFile.Id && p.ArchiveId == archiveID).ToList();

            if (vm.Marks == null || vm.Marks.Count == 0)
                return;

            parameters.Add("ExportViewModel", vm);
            parameters.Add("FileName", Model.noteFile.NoteFileName + (true ? ".html" : ".txt"));

            Modal.OnClose += HideDialogOutput;
            Modal.Show<ExportUtil1>("", parameters);
        }

        void HideDialogOutput(ModalResult modalResult)
        {
            Modal.OnClose -= HideDialogOutput;
            OutputCleanup();
        }

        protected async Task OutputCleanup()
        {
            List<Mark> marks = await Http.GetJsonAsync<List<Mark>>("api/Mark");

            List<Mark> FileMarks = marks.Where(p => p.NoteFileId == Model.noteFile.Id && p.ArchiveId == archiveID).ToList();

            await Http.DeleteAsync("api/Mark/" + FileMarks[0].NoteFileId + "." + archiveID);

            Model = await Http.GetJsonAsync<NoteDisplayIndexModel>("api/NoteIndex/" + Id + "." + archiveID);
            Model.Notes = Model.AllNotes.Where(p => p.ResponseOrdinal == 0).OrderBy(p => p.NoteOrdinal).ToList();

            StateHasChanged();
        }

        /// <summary>
        /// Browse image datebase
        /// </summary>
        protected void Browse()
        {
            StopTimer();
            var parameters = new ModalParameters();
            Modal.OnClose += HideDialog;
            Modal.Show<ImageDlg>("", parameters);
        }

        protected class LocalInput
        {
            [StringLength(10)]
            public string typedValue { get; set; }
            public bool isAutoFocus { get; set; }
        }
        #endregion

    }
}
