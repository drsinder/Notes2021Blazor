using Microsoft.AspNetCore.Components;
using Notes2021Blazor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.JSInterop;
using Blazored.Modal;
using Blazored.Modal.Services;
using System.Text;
using System.Timers;

namespace Notes2021Spa.RCL.Pages.Notes
{
    public class DisplayPanelComp : ComponentBase
    {
        [Inject]
        HttpClient Http { get; set; }
        [Inject]
        IJSRuntime jsRuntime { get; set; }
        [Inject]
        IModalService Modal { get; set; }

        [Parameter] public NoteDisplayIndexModel Model { get; set; }
        [Parameter] public NoteHeader currentHeader { get; set; }
        [Parameter] public int Id { get; set; }
        [Parameter] public List<NoteHeader> SeqBases { get; set; }
        [Parameter] public List<Sequencer> trackers { get; set; }
        [Parameter] public int seqIndx { get; set; }
        [Parameter] public EventCallback<string> OnClick { get; set; }
        [Parameter] public bool isSearch { get; set; }
        [Parameter] public List<NoteHeader> SearchResults { get; set; }
        [Parameter] public bool PrintFile { get; set; } = false;  // set to true by parent to print the file
        [Parameter] public bool FocusTimer { get; set; } = true;  // set to false by parent to inhibit focus timer
        [Parameter] public bool ToolTips { get; set; }            // false if in expanded note

        public bool SubNotes { get; set; } = false;

        protected bool newnoteFlag { get; set; }
        protected bool editing { get; set; }

        protected Timer timer { get; set; }
        protected Timer timer2 { get; set; }

        public string respX { get; set; }

        public NoteContent currentContent { get; set; }

        public List<Tags> tags;

        protected string curN { get; set; }

        protected LocalInput myInput { get; set; }

        protected int seqBaseIndx { get; set; }
        protected int seqRespOrd { get; set; }

        protected double baseNotes { get; set; }
        protected double currNote { get; set; }

        protected string sliderValueText { get; set; }

        protected bool Multiple { get; set; }

        protected bool Expandable { get; set; }

        /// <summary>
        /// Initialize everything
        /// </summary>
        /// <returns></returns>
        protected override async Task OnParametersSetAsync()
        {
            Multiple = Model.UserData.Pref4;
            Expandable = Model.UserData.Pref5;
            Model.myHeader = currentHeader;
            myInput = new LocalInput();
            seqBaseIndx = 0;
            seqRespOrd = 0;
            SubNotes = Model.UserData.Pref3;
            baseNotes = Model.Notes.Count;
            currNote = 1;
            sliderValueText = "1/" + baseNotes;

            newnoteFlag = false;
            editing = false;

            if (isSearch && SearchResults.Count > 0)
            {
                currentHeader = SearchResults[0];
                Model.myHeader = currentHeader;
            }

            // get content and tags for this note - also includes director message
            DisplayModel dm = await Http.GetJsonAsync<DisplayModel>("api/NoteContent/" + currentHeader.Id);

            currentContent = dm.content;
            tags = dm.tags;

            // set text to be displayed re responses
            respX = "";
            if (currentHeader.ResponseCount > 0)
                respX = " - " + currentHeader.ResponseCount + " Responses ";
            else if (currentHeader.ResponseOrdinal > 0)
                respX = " Response " + currentHeader.ResponseOrdinal;

            // set value to be displayed in navigation box
            curN = "" + currentHeader.NoteOrdinal;
            if (currentHeader.ResponseOrdinal > 0)
            {
                curN += "." + currentHeader.ResponseOrdinal;
            }

            if (PrintFile)  // print whole file
            {
                await PrintString();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && ToolTips) // we can not set focus to the nav box right away so defer it 1 second
            {
                if (FocusTimer)
                {
                    timer = new Timer(1000);
                    timer.Elapsed += TimerTick;
                    timer.Enabled = true;
                }
            }
            // highlight code sections (if any) of a note
            await jsRuntime.InvokeVoidAsync("Prism.highlightAll");
        }

        protected void TimerTick(Object source, ElapsedEventArgs e)
        {
            timer.Interval = 5000;
            timer.Stop();
            timer.Enabled = false;
            jsRuntime.InvokeAsync<object>("setfocus", "arrow2");
        }

        /// <summary>
        /// Print a single note
        /// </summary>
        protected void Print()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<h4 class=\"text-center\">" + Model.noteFile.NoteFileTitle + "</h4>");
            sb.Append("<div class=\"noteheader\"><p> <span class=\"keep-right\">Note: ");
            sb.Append(currentHeader.NoteOrdinal + " " + respX);
            sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;</span></p><h4>Subject: ");
            sb.Append(@currentHeader.NoteSubject);
            sb.Append("<br />Author: ");
            sb.Append((Globals.LocalTimeBlazor(currentHeader.LastEdited).ToLongDateString()) + " " + (Globals.LocalTimeBlazor(currentHeader.LastEdited).ToShortTimeString())/* + " " + Model.tZone.Abbreviation*/);
            if (!string.IsNullOrEmpty(currentContent.DirectorMessage))
            {
                sb.Append("<br /><span>Director Message: ");
                sb.Append(currentContent.DirectorMessage);
                sb.Append("</span>");
            }
            if (tags != null && tags.Count > 0)
            {
                sb.Append(" <br /><span>Tags: ");
                foreach (Tags tag in tags)
                {
                    sb.Append(tag.Tag + " ");
                }
                sb.Append("</span>");
            }
            sb.Append("</h4></div><div class=\"notebody\" >");
            sb.Append(currentContent.NoteBody);
            sb.Append("</div>");
            string stuff = sb.ToString();

            var parameters = new ModalParameters();
            parameters.Add("PrintStuff", stuff);
            Modal.OnClose += HideDialog;
            Modal.Show<PrintDlg>("", parameters);
        }

        /// <summary>
        /// Print a note string (or whole file if PrintFile is true
        /// </summary>
        protected async Task PrintString()
        {
            // keep track of base note
            NoteHeader baseHeader = currentHeader;

            StringBuilder sb = new StringBuilder();

            sb.Append("<h4 class=\"text-center\">" + Model.noteFile.NoteFileTitle + "</h4>");

        reloop: // come back here to do another note

            sb.Append("<div class=\"noteheader\"><p> <span class=\"keep-right\">Note: ");
            sb.Append(currentHeader.NoteOrdinal + " " + respX);
            sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;</span></p><h4>Subject: ");
            sb.Append(currentHeader.NoteSubject);
            sb.Append("<br />Author: ");
            sb.Append(currentHeader.AuthorName + "    ");
            sb.Append((Globals.LocalTimeBlazor(currentHeader.LastEdited).ToLongDateString()) + " " + (Globals.LocalTimeBlazor(currentHeader.LastEdited).ToShortTimeString())/* + " " + Model.tZone.Abbreviation*/);
            if (!string.IsNullOrEmpty(currentContent.DirectorMessage))
            {
                sb.Append("<br /><span>Director Message: ");
                sb.Append(currentContent.DirectorMessage);
                sb.Append("</span>");
            }
            if (tags != null && tags.Count > 0)
            {
                sb.Append(" <br /><span>Tags: ");
                foreach (Tags tag in tags)
                {
                    sb.Append(tag.Tag + " ");
                }
                sb.Append("</span>");
            }
            sb.Append("</h4></div><div class=\"notebody\" >");
            sb.Append(currentContent.NoteBody);
            sb.Append("</div>");

            if (currentHeader.ResponseOrdinal < baseHeader.ResponseCount) // more responses in string
            {
                currentHeader = Model.AllNotes.Single(p => p.NoteOrdinal == currentHeader.NoteOrdinal && p.ResponseOrdinal == currentHeader.ResponseOrdinal + 1);
                await SetNote();    // set important stuff
                goto reloop;        // print another note
            }

            currentHeader = baseHeader; // set back to base note
            await SetNote();            // set important stuff

            if (PrintFile)  // whole file printing
            {
                NoteHeader next = Model.Notes.SingleOrDefault(p => p.NoteOrdinal == currentHeader.NoteOrdinal + 1);
                if (next != null)       // still base notes left to print
                {
                    currentHeader = next;   // set current note and base note
                    baseHeader = next;
                    await SetNote();        // set important stuff
                    sliderValueText = currentHeader.NoteOrdinal + "/" + baseNotes;  // update progress test
                    currNote = currentHeader.NoteOrdinal;                           // update progress bar

                    goto reloop;    // print another string
                }
            }

            string stuff = sb.ToString();           // turn accumulated output into a string

            var parameters = new ModalParameters();
            parameters.Add("PrintStuff", stuff);    // pass string to print dialog
            Modal.OnClose += HidePrintDialog;
            Modal.Show<PrintDlg>("", parameters);   // invloke print dialog with our output
        }

        protected void StartTimer()
        {
            if (!FocusTimer)
                return;
            timer = new Timer(1000);
            timer.Elapsed += TimerTick;
            timer.Enabled = true;
        }

        protected void StopTimer()
        {
            if (!FocusTimer)
            {
                // try to kill parent focus timer
                OnClick.InvokeAsync("StopTimer");
                return;
            }
            timer.Enabled = false;
            timer.Stop();
        }

        /// <summary>
        /// Handles messages from children
        /// </summary>
        /// <param name="newMessage">Text of message</param>
        public async Task ClickHandler(string newMessage)
        {
            switch (newMessage)
            {
                case "ShowHelp": ShowHelp(); break;
                case "NextBaseNote": await NextBaseNote(); break;
                case "PreviousBaseNote": await PreviousBaseNote(); break;
                case "NextNote": await TextHasChanged(""); break;
                case "PreviousNote": await PreviousNote(); break;
                case "NewResponse":
                    await NewResponse();
                    break;
                case "Edit":
                    await EditNote();
                    //editing = true;
                    //this.StateHasChanged();
                    break;
                case "Delete": DeleteNote(); break;
                case "Forward": Forward(); break;
                case "Done": Done(); break;
                case "Html": Html(); break;
                case "html": html(); break;
                case "mail": mail(); break;
                case "Search":
                    StopTimer();
                    await OnClick.InvokeAsync(newMessage);
                    break;
                case "Mark":
                    await Markit();
                    break;
                case "StopTimer":
                    StopTimer();
                    break;
                case "CancelEdit":
                    newnoteFlag = editing = false;
                    StateHasChanged();
                    break;
                case "Copy":
                    Copy(currentHeader);
                    break;

                default:
                    await OnClick.InvokeAsync(newMessage);
                    break;
            }
        }

        protected void Copy(NoteHeader Note)
        {
            if (!Model.myAccess.ReadAccess)
                return;

            StopTimer();
            var parameters = new ModalParameters();
            parameters.Add("Note", Note);
            parameters.Add("UserData", Model.UserData);
            Modal.OnClose += HideDialog;
            Modal.Show<Copy>("", parameters);
        }

        protected async Task NewResponse()
        {
            StopTimer();
            newnoteFlag = true;
            StateHasChanged();
        }

        protected async Task EditNote()
        {
            StopTimer();
            editing = true;
            StateHasChanged();
        }

        protected void DeleteNote()
        {
            var parameters = new ModalParameters();
            parameters.Add("NoteId", currentHeader.Id);
            parameters.Add("FileId", Model.noteFile.Id);
            var options = new ModalOptions() { HideCloseButton = false };
            Modal.OnClose += HideDeleteDialog;
            Modal.Show<DeleteNote>("", parameters, options);
        }

        /// <summary>
        /// Handles keys pressed in navigation box so single key strokes can invoke a function
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        protected async Task KeyPressed(Microsoft.AspNetCore.Components.Web.KeyboardEventArgs eventArgs)
        {
            switch (eventArgs.Key)
            {
                case "Z": ShowHelp(); break;
                case "E":
                    if (@Model.myAccess.DeleteEdit || Model.myHeader.AuthorID == Model.myAccess.UserID)
                        await EditNote();
                    //    editing = true;
                    //StateHasChanged();
                    break;
                case "D":
                    if (@Model.myAccess.DeleteEdit || Model.myHeader.AuthorID == Model.myAccess.UserID)
                        DeleteNote();
                    break;
                case "Enter":
                    if (!eventArgs.ShiftKey)
                        await TextHasChanged("");   // next note
                    else
                        await NextBaseNote();
                    break;
                case "b":
                    await PreviousNote();
                    break;
                case "B":
                    await PreviousBaseNote();
                    break;
                case "N":
                    await NewResponse();
                    break;
                case "L":
                case "I":
                    Done();      // we are dome here...
                    break;
                case " ":
                    if (eventArgs.ShiftKey)
                    {
                        if (!isSearch)
                            return;

                        SearchResults.RemoveAt(0);
                        if (SearchResults.Count == 0)
                        {
                            ShowMessage("Search Complete!");
                            isSearch = false;
                            return;
                        }

                        currentHeader = SearchResults[0];
                        await SetNote();
                    }
                    else
                    {
                        if (seqIndx == -1)
                            break;

                        await ContinueSeq();
                    }
                    break;
                case "F":
                    Forward();
                    break;
                case "H":
                    Html();
                    break;
                case "h":
                    html();
                    break;
                case "m":
                    mail();
                    break;
                case "S":
                    StopTimer();
                    await OnClick.InvokeAsync("Search");
                    break;
                case "M":
                    await Markit();
                    break;
                case "U":
                    Browse();
                    break;
                case "C":
                    Copy(currentHeader);
                    break;

                default:
                    //var parameters = new ModalParameters();
                    //parameters.Add("Message", eventArgs.Key);
                    //var options = new ModalOptions() { HideCloseButton = false };
                    //Modal.OnClose += HideDialog;
                    //Modal.Show<MessageBox>("", parameters, options);
                    break;
            }
        }

        protected async Task ContinueSeq()  // continue sequencing - tracking new notes
        {
            // first check for responses
            NoteHeader x = Model.AllNotes.Where(p => p.NoteOrdinal == currentHeader.NoteOrdinal
                    && p.ResponseOrdinal > currentHeader.ResponseOrdinal
                    && p.LastEdited > trackers[seqIndx].LastTime)
                    .OrderBy(p => p.ResponseOrdinal).FirstOrDefault();

            if (x != null) // found a response
            {
                currentHeader = x;
                await SetNote();
                return;
            }
            // else goto next base
            seqBaseIndx++;
            if (seqBaseIndx >= SeqBases.Count)  // finished with this file
            {
                await OnClick.InvokeAsync("SeqFileDone");
                return;
            }
            currentHeader = SeqBases[seqBaseIndx];
            await SetNote();
        }

        /// <summary>
        /// Given the currentHeader, set up to display or print a note
        /// </summary>
        protected async Task SetNote()
        {
            // get NoteContent entity and list of tags
            DisplayModel dm = await Http.GetJsonAsync<DisplayModel>("api/NoteContent/" + currentHeader.Id);

            currentContent = dm.content;
            tags = dm.tags;

            // store stuff to display re responses
            respX = "";
            if (currentHeader.ResponseOrdinal > 0)
            {
                NoteHeader bnh = Model.Notes.Find(p => p.Id == currentHeader.BaseNoteId);
                respX = " - Response " + currentHeader.ResponseOrdinal + " of " + bnh.ResponseCount;
            }
            else if (currentHeader.ResponseCount > 0)
                respX = " - " + currentHeader.ResponseCount + " Responses ";

            // set nav box info
            curN = "" + currentHeader.NoteOrdinal;
            if (currentHeader.ResponseOrdinal > 0)
            {
                curN += "." + currentHeader.ResponseOrdinal;
            }

            // stuff currentHeader into the Model for tracking
            Model.myHeader = currentHeader;
            this.StateHasChanged();     // tell blazor things have changed
        }

        protected async Task PreviousBaseNote()
        {
            if (currentHeader.NoteOrdinal == 1)
                return;
            currentHeader = Model.Notes.Find(p => p.NoteOrdinal == currentHeader.NoteOrdinal - 1 && p.ResponseOrdinal == 0);
            if (currentHeader == null)
                return;
            await SetNote();
        }

        protected async Task NextBaseNote()
        {
            NoteHeader newbase = Model.Notes.Find(p => p.NoteOrdinal == currentHeader.NoteOrdinal + 1 && p.ResponseOrdinal == 0);
            if (newbase == null)
                return;
            currentHeader = newbase;
            await SetNote();
        }

        protected async Task PreviousNote()
        {
            if (currentHeader.ResponseOrdinal == 0)
            {
                await PreviousBaseNote();
                return;
            }
            NoteHeader newbase = Model.AllNotes.Find(p => p.NoteOrdinal == currentHeader.NoteOrdinal && p.ResponseOrdinal == currentHeader.ResponseOrdinal - 1);
            if (newbase == null)
                return;

            currentHeader = newbase;
            await SetNote();
        }

        protected void Done()
        {
            OnClick.InvokeAsync("Done:" + currentHeader.NoteOrdinal);   // tell parent I'm done
        }

        protected void DoneToIndex()
        {
            OnClick.InvokeAsync("Done");   // tell parent I'm done
        }

        /// <summary>
        /// Handle typing in navigation box. Format of actions:
        /// n = goto note n
        /// n.r goto note n response r
        /// +n go forward n notes
        /// -n go backward n notes
        /// +.r go forward r responses
        /// -.r go backward r responses
        /// </summary>
        /// <param name="typedInput">content of nav box</param>
        protected async Task TextHasChanged(string typedInput)
        {
            int fileId = Id;
            int iOrd = currentHeader.NoteOrdinal;
            int iResp = currentHeader.ResponseOrdinal;
            long iNoteId = currentHeader.Id;
            int noteOrd = 1;
            NoteHeader nc;
            NoteHeader bnh = Model.Notes.Find(p => p.Id == currentHeader.BaseNoteId);
            bool ax = false;
            bool plus = false;
            bool minus = false;

            // strip cruft
            typedInput = typedInput.Trim().Replace("'\n", "").Replace("'\r", "").Trim();

            // if empty - next note
            if (string.IsNullOrEmpty(typedInput) || string.IsNullOrWhiteSpace(typedInput))
            {
                // next note
                NoteHeader searcher = Model.AllNotes.Find(p => p.NoteOrdinal == currentHeader.NoteOrdinal && p.ResponseOrdinal == currentHeader.ResponseOrdinal + 1);
                if (searcher != null)  // found a response
                {
                    nc = searcher;
                    bnh = Model.Notes.Find(p => p.NoteOrdinal == currentHeader.NoteOrdinal);

                    goto beyond;    // go execute our results
                }
                searcher = Model.Notes.Find(p => p.NoteOrdinal == currentHeader.NoteOrdinal + 1);
                if (searcher != null) // found a base
                {
                    nc = bnh = searcher;
                    goto beyond;    // go execute our results
                }
                Done();  // reached the end - we be done - tell parent
            }

            // check for starting with + or -
            if (typedInput.StartsWith("+"))
                plus = true;
            else if (typedInput.StartsWith("-"))
                minus = true;

            // strip + and -
            typedInput = typedInput.Replace("+", "").Replace("-", "");

            // check for a .
            if (typedInput.Contains("."))
            {
                string[] splits = typedInput.Split(new[] { '.' });
                if (splits.Length != 2) // not exactly 1 .
                {
                    return; // ignore
                }
                if (string.IsNullOrEmpty(splits[0]) || string.IsNullOrWhiteSpace(splits[0]))
                    noteOrd = iOrd; // stay where we are
                else   // compute note ordinal
                    ax = !int.TryParse(splits[0], out noteOrd);
                // compute response ordinal
                bool bx = !int.TryParse(splits[1], out var respOrd);
                if (ax || bx) // one or the other was bad - ignore
                {
                    return;
                }

                if (noteOrd == iOrd && (plus || minus)) // if no note ordinal change and user used + or - prefix
                {
                    if (plus)       // bump response ordinal by requested amount
                        respOrd += iResp;
                    else            //  go back requested amount (responses)
                        respOrd = iResp - respOrd;

                    if (respOrd < 0)  // lower bound check
                        respOrd = 0;
                    bnh = Model.Notes.Find(p => p.NoteOrdinal == noteOrd);

                    if (respOrd > bnh.ResponseCount)    // upper bound check
                        respOrd = bnh.ResponseCount;
                }

                // get that note header to a temp
                nc = Model.AllNotes.Find(p => p.NoteOrdinal == noteOrd && p.ResponseOrdinal == respOrd);
            }
            else // no response part given (no . entered) - work on base notes
            {
                if (!int.TryParse(typedInput, out noteOrd))
                {
                    return;
                }

                if (!plus && !minus && (noteOrd == 0))
                {
                    Done();     // user entered 0 - we be done - tell parent
                }

                // if prefixed with + or - then add or subtract number typed
                if (plus)
                    noteOrd += iOrd;
                else if (minus)
                    noteOrd = iOrd - noteOrd;

                // lower bounds check
                if (noteOrd < 1)
                    noteOrd = 1;

                long cnt = Model.Notes.LongCount();  // get count of notes

                // upper bounds check
                if (noteOrd > cnt)
                    noteOrd = (int)cnt;

                // get the base note header
                nc = Model.Notes.Find(p => p.NoteOrdinal == noteOrd);
                bnh = nc;  // set base and temp
            }

            if (nc == null)
            {
                return;
            }

        beyond:

            // set current note from temp
            currentHeader = nc;

            await SetNote();  // set important stuff before we go on
        }

        protected void ShowMessage(string message)
        {
            var parameters = new ModalParameters();
            parameters.Add("Message", message);
            Modal.OnClose += HideDialog;
            Modal.Show<MessageBox>("", parameters);
        }

        protected void ShowHelp()
        {
            var parameters = new ModalParameters();
            Modal.OnClose += HideDialog;
            var options = new ModalOptions() { HideCloseButton = false };
            Modal.Show<HelpDialog2>("", parameters, options);
        }

        void HideDialog(ModalResult modalResult)
        {
            Modal.OnClose -= HideDialog;
            StartTimer();
        }

        void HideDeleteDialog(ModalResult modalResult)
        {
            Modal.OnClose -= HideDeleteDialog;
            StartTimer();
            if (modalResult.Cancelled)
                return;
            DoneToIndex();
        }

        void HidePrintDialog(ModalResult modalResult)
        {
            Modal.OnClose -= HidePrintDialog;
            if (PrintFile)
            {
                Done();
            }
        }

        protected void Forward()
        {
            StopTimer();
            var parameters = new ModalParameters();
            ForwardViewModel fv = new ForwardViewModel();
            fv.NoteID = currentHeader.Id;
            fv.FileID = currentHeader.NoteFileId;
            fv.ArcID = currentHeader.ArchiveId;
            fv.NoteOrdinal = currentHeader.NoteOrdinal;
            fv.NoteSubject = currentHeader.NoteSubject;

            if (currentHeader.ResponseCount > 0 || currentHeader.BaseNoteId > 0)
                fv.hasstring = true;

            parameters.Add("ForwardView", fv);

            Modal.OnClose += HideDialog;
            var options = new ModalOptions() { HideCloseButton = false };
            Modal.Show<Forward>("", parameters, options);
        }

        protected void Html()
        {
            OnClick.InvokeAsync("Html:" + currentHeader.NoteOrdinal);
        }

        protected void html()
        {
            OnClick.InvokeAsync("html:" + currentHeader.NoteOrdinal);
        }

        protected void mail()
        {
            StopTimer();
            OnClick.InvokeAsync("mail:" + currentHeader.NoteOrdinal);
        }

        /// <summary>
        /// Mark note string for output
        /// </summary>
        protected async Task Markit()
        {
            // get the current marks for this user
            List<Mark> list = await Http.GetJsonAsync<List<Mark>>("api/Mark");

            // find those for this note
            if (list != null && list.Count > 0)
            {
                List<Mark> list2 = list.Where(p => p.NoteFileId == currentHeader.NoteFileId
                    && p.ArchiveId == currentHeader.ArchiveId
                    && p.NoteOrdinal == currentHeader.NoteOrdinal)
                    .ToList();

                if (list2 != null && list2.Count > 0)
                {
                    ShowMessage("Already Marked for Output");
                    return;
                }
            }

            // prepare a new mark
            Mark myMark = new Mark();
            myMark.UserId = "Temp";
            myMark.NoteFileId = currentHeader.NoteFileId;
            myMark.ArchiveId = currentHeader.ArchiveId;
            myMark.NoteHeaderId = currentHeader.Id;
            myMark.NoteOrdinal = currentHeader.NoteOrdinal;
            myMark.MarkOrdinal = 1;         // sloppy for now - does not permit outputing in marked order
            myMark.ResponseOrdinal = -1;    // also sloppy for now -
                                            // we are marking the whole string rather than allowing selection

            // send to the server for storage
            await Http.PostJsonAsync("api/Mark", myMark);

            Model.isMarked = true;

            ShowMessage("Marked for Output");
        }

        protected void Browse()
        {
            StopTimer();
            var parameters = new ModalParameters();
            Modal.OnClose += HideDialog;
            Modal.Show<ImageDlg>("", parameters);
        }


        protected class LocalInput
        {
            public string typedValue { get; set; }
        }



    }
}
