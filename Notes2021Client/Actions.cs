using Newtonsoft.Json;
using Notes2021Blazor.Shared;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace Notes2021Client
{
     /// <summary>
    /// Api Calls for accessing notesfiles
    /// </summary>
    public static class Actions
    {
        /// <summary>
        /// Attempt to log use in
        /// </summary>
        /// <param name="MyClient">Initialized HttpClient - Set
        /// MyClient.BaseAddress = new Uri(baseUri); for address (https)
        /// of server
        /// </param>
        /// <param name="email">login email</param>
        /// <param name="password">password</param>
        /// <returns>An HttpResponseMessage - If StatusCode == HttpStatusCode.OK
        /// the login was successfull.  Then
        /// token = resp.Content.ReadAsStringAsync().Result;
        /// And save the token for the session.  Also you must
        /// Add the token to the client headers:
        /// MyClient.DefaultRequestHeaders.Add("authentication", token);
        /// </returns>
        public static HttpResponseMessage Login(HttpClient MyClient, string email, string password )
        {
            return MyClient.PostAsync("api/ApiLogin/", 
                new StringContent(email + "/" + password)).GetAwaiter().GetResult();
        }


        /// <summary>
        /// Gets a list of files thre user is permitted access to
        /// </summary>
        /// <param name="MyClient">Populated HttpClient</param>
        /// <returns>File List</returns>
        public static List<NoteFile> GetFileList(HttpClient MyClient)
        {
            HttpResponseMessage response = MyClient.GetAsync("api/ApiNotes").Result;
            var value = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            List<NoteFile> obj = JsonConvert.DeserializeObject<List<NoteFile>>(value);
            return obj;
        }

        /// <summary>
        /// Gets a list of base notes for display on index
        /// </summary>
        /// <param name="MyClient">Populated HttpClient</param>
        /// <param name="fileId">Id of target flile</param>
        /// <returns>List of Note Headers</returns>
        public static List<NoteHeader> GetNoteList(HttpClient MyClient, int fileId)
        {
            HttpResponseMessage response = MyClient.GetAsync("api/ApiNotes/" + fileId).Result;
            return response.Content.ReadAsAsync<List<NoteHeader>>().Result;
        }

        /// <summary>
        /// Gets the users access token for the file
        /// </summary>
        /// <param name="MyClient">Populated HttpClient</param>
        /// <param name="fileId">Id of file</param>
        /// <returns>NoteAccess object for the user in the file</returns>
        public static NoteAccess GetAccess(HttpClient MyClient, int fileId)
        {
            HttpResponseMessage response2 = MyClient.GetAsync("api/ApiLogin/" + fileId).Result;
            return response2.Content.ReadAsAsync<NoteAccess>().Result;
        }

        /// <summary>
        /// Exports as html or text
        /// </summary>
        /// <param name="MyClient">Populated HttpClient</param>
        /// <param name="fileId">file id to export</param>
        /// <param name="isHtml">true for html</param>
        /// <returns>Stream containing output</returns>
        public static Stream Export(HttpClient MyClient, int fileId, bool isHtml)
        {
            HttpResponseMessage response = MyClient.GetAsync("api/ApiContent/" + fileId
                                + "/" + isHtml).Result;

            return response.Content.ReadAsStreamAsync().Result;
        }

        /// <summary>
        /// Create a new note
        /// </summary>
        /// <param name="MyClient">Populated HttpClient</param>
        /// <param name="tv">A populated TextViewModel</param>
        /// <returns>HttpResponseMessage</returns>
        public static HttpResponseMessage CreateNote(HttpClient MyClient, TextViewModel tv)
        {
            tv.NoteID = 0;
            return MyClient.PostAsync("api/ApiContent/",
                new ObjectContent(typeof(TextViewModel), tv, new JsonMediaTypeFormatter())).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Edits and existing note
        /// </summary>
        /// <param name="MyClient">Populated HttpClient</param>
        /// <param name="tv">A Populated TextViewModel</param>
        /// <param name="headerId">Id of the note you are editing</param>
        /// <returns>HttpResponseMessage</returns>
        public static HttpResponseMessage EditNote(HttpClient MyClient, TextViewModel tv, long headerId)
        {
            tv.NoteID = headerId;
            return MyClient.PutAsync("api/ApiContent/",
                new ObjectContent(typeof(TextViewModel), tv, new JsonMediaTypeFormatter())).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets header and response headers for a base note
        /// </summary>
        /// <param name="MyClient">Populated HttpClient</param>
        /// <param name="fileId">target file Id</param>
        /// <param name="noteOrdinal">Note Ordinal in file</param>
        /// <returns>List of Note Headers</returns>
        public static IEnumerable<NoteHeader> GetDislayNoteHeadersWithResponses(HttpClient MyClient, int fileId, int noteOrdinal)
        {
            HttpResponseMessage response = MyClient.GetAsync("api/ApiNotes/" + fileId + "/" + noteOrdinal).Result;
            return response.Content.ReadAsAsync<IEnumerable<NoteHeader>>().Result;
        }

        /// <summary>
        /// Gets the content for a note
        /// </summary>
        /// <param name="MyClient">Populated HttpClient</param>
        /// <param name="fileId">target file Id</param>
        /// <param name="baseOrdinal">Base note Ordinal</param>
        /// <param name="response">Response number - zero for base</param>
        /// <returns>NoteContent Object</returns>
        public static NoteContent GetNoteContent(HttpClient MyClient, int fileId, int baseOrdinal, int response)
        {
            HttpResponseMessage response2 = MyClient.GetAsync("api/ApiNotes/" + fileId
                      + "/" + baseOrdinal + "/" + response).Result;
            return response2.Content.ReadAsAsync<NoteContent>().Result;
        }


        /// <summary>
        /// Gets the collection of Tags for a note
        /// </summary>
        /// <param name="MyClient">Populated HttpClient</param>
        /// <param name="fileId">Target file Id</param>
        /// <param name="baseOrdinal">Ordinal of base note</param>
        /// <param name="response">Response number</param>
        /// <returns>Collection of Tags</returns>
        public static IEnumerable<Tags> GetTags(HttpClient MyClient, int fileId, int baseOrdinal, int response)
        {
            HttpResponseMessage response3 = MyClient.GetAsync("api/ApiNotes/" + fileId
                         + "/" + baseOrdinal + "/" + response + "/0").Result;

            return response3.Content.ReadAsAsync<IEnumerable<Tags>>().Result;
        }

        /// <summary>
        /// Deletes a note
        /// </summary>
        /// <param name="MyClient">Populated HttpClient</param>
        /// <param name="Id">Header Id for note to delete</param>
        public static void DeleteNote(HttpClient MyClient, long Id)
        {
            MyClient.DeleteAsync("api/ApiNotes/" + Id);

        }
    }
}
