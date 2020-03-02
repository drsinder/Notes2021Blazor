using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Notes2021Blazor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using Blazored.LocalStorage;
using System.Timers;
using Syncfusion.EJ2.Blazor.Spinner;
using System.Security.Claims;

namespace Notes2021Spa.RCL.Pages.Root
{
    public class HomeComp : ComponentBase
    {
        [Inject]
        HttpClient Http { get; set; }
        [Inject]
        ILocalStorageService _localStorage { get; set; }
        [Inject]
        AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject]
        IAuthorizationService AuthorizationService { get; set; }
        [Inject]
        IJSRuntime jsRuntime { get; set; }

        [Parameter] public EventCallback<Message> OnMessage { get; set; }
        public List<localFile> fileList { get; set; }
        public List<localFile> nameList { get; set; }
        public localFile dummyFile { get; set; }
        public List<localFile> impfileList { get; set; }
        public List<localFile> histfileList { get; set; }

        protected HomePageModel model;
        protected Timer myTimer;
        protected string myTime;

        protected EjsSpinner SpinnerObj;
        string target { get; set; } = "#container";

        [CascadingParameter]
        protected Task<AuthenticationState> authenticationStateTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Globals.RootUri = ""; // NavigationManager.Uri;

            ClaimsPrincipal user = (await authenticationStateTask).User;

            await _localStorage.SetItemAsync("ArchiveId", 0);
            await _localStorage.RemoveItemAsync("IsSearch");
            await _localStorage.RemoveItemAsync("ReturnUri");

            if (user.Identity.IsAuthenticated)
            {
                HomePageModel model1 = await Http.GetJsonAsync<HomePageModel>("api/HomePageData");

                Globals.GuestId = model1.GuestId;
                Globals.GuestEmail = model1.GuestEmail;

                myTime = Globals.LocalTimeBlazor(DateTime.Now.ToUniversalTime()).ToLongTimeString()
                    + /*" " + model1.TimeZone.Abbreviation +*/ " - "
                    + Globals.LocalTimeBlazor(DateTime.Now.ToUniversalTime()).ToLongDateString();

                myTimer = new Timer(1000);
                myTimer.Elapsed += OnTimedEvent;
                myTimer.AutoReset = true;
                myTimer.Enabled = true;

                model = model1;

                dummyFile = new localFile { Id = 0, NoteFileName = " ", NoteFileTitle = " " };
                fileList = new List<localFile>();
                nameList = new List<localFile>();
                histfileList = new List<localFile>();
                impfileList = new List<localFile>();

                List<NoteFile> fileList1 = model.NoteFiles.OrderBy(p => p.NoteFileName).ToList();
                List<NoteFile> nameList1 = model.NoteFiles.OrderBy(p => p.NoteFileTitle).ToList();


                for (int i = 0; i < fileList1.Count; i++)
                {
                    localFile work = new localFile { Id = fileList1[i].Id, NoteFileName = fileList1[i].NoteFileName, NoteFileTitle = fileList1[i].NoteFileTitle };
                    localFile work2 = new localFile { Id = nameList1[i].Id, NoteFileName = nameList1[i].NoteFileName, NoteFileTitle = nameList1[i].NoteFileTitle };
                    fileList.Add(work);
                    nameList.Add(work2);

                    string fname = work.NoteFileName;
                    if (fname == "Opbnotes" || fname == "Gnotes")
                        histfileList.Add(work);

                    if (fname == "announce" || fname == "pbnotes" || fname == "noteshelp")
                        impfileList.Add(work);
                }
            }
            else
            {
                HomePageModel model1 = await Http.GetJsonAsync<HomePageModel>("api/HomePageData");

                Globals.GuestId = model1.GuestId;
                Globals.GuestEmail = model1.GuestEmail;

                model = new HomePageModel();
                model.Message = model1.Message;
                fileList = nameList = new List<localFile>();
                myTime = "";
            }

        }

        protected override void OnAfterRender(bool firstRender)
        {
            jsRuntime.InvokeAsync<object>("setselect0", "select1");
            jsRuntime.InvokeAsync<object>("setselect0", "select2");
        }

        protected void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            myTime = Globals.LocalTimeBlazor(DateTime.Now.ToUniversalTime()).ToLongTimeString()
                + /*" " + model.TimeZone.Abbreviation + */" - "
                + Globals.LocalTimeBlazor(DateTime.Now.ToUniversalTime()).ToLongDateString();

            this.StateHasChanged();
        }

        protected void TextHasChanged(string value)
        {
            value = value.Trim().Replace("'\n", "").Replace("'\r", "");

            try
            {
                foreach (var item in fileList)
                {
                    if (value == item.NoteFileName)
                    {
                        SpinnerObj.ShowSpinner(target);
                        OnMessage.InvokeAsync(new Message { Command = Commands.Show, Location = Locations.FileUserIndex, IntArg = item.Id });
                        return;
                    }
                }
            }
            catch
            { }
        }

        protected void MessageHandler(Message message)
        {
            SpinnerObj.ShowSpinner(target);
            OnMessage.InvokeAsync(message);
        }

        protected void Autologin()
        {
            OnMessage.InvokeAsync(new Message { Command = Commands.Dialog, Location = Locations.Login, StringArg = Globals.GuestEmail });
        }

        public class localFile : NoteFile
        {
        }
    }
}
