/*--------------------------------------------------------------------------
    **
    ** Copyright(c) 2020, Dale Sinder
    **
    ** Name: EmailSender.cs
    **
    ** Description:
    **      Send Email sometimes with attachment
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


using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IO;
using System.Threading.Tasks;

namespace Notes2021Blazor.Server.Services
{
    public class EmailSender : IEmailSender
    {
        public StreamWriter StreamWriter { get; private set; }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var apiKey = Globals.SendGridApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(Globals.SendGridEmail, Globals.EmailName);
            var to = new EmailAddress(email);
            var htmlStart = "<!DOCTYPE html>";
            var isHtml = message.StartsWith(htmlStart);
            SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, isHtml ? "See Html." : message, message);

            if (isHtml)
            {
                MemoryStream ms = new MemoryStream();
                StreamWriter sw = new StreamWriter(ms);
                await sw.WriteAsync(message);
                await sw.FlushAsync();
                ms.Seek(0, SeekOrigin.Begin);
                await msg.AddAttachmentAsync("FromNotes2021.html", ms);
                ms.Dispose();
            }

            Response response = await client.SendEmailAsync(msg);
        }

        //public async Task SendEmailListAsync(List<string> emails, string subject, string message)
        //{
        //    var apiKey = Globals.SendGridApiKey;
        //    var client = new SendGridClient(apiKey);
        //    var from = new EmailAddress(Globals.SendGridEmail, Globals.EmailName);
        //    List<EmailAddress> tos = new List<EmailAddress>();

        //    foreach (string item in emails)
        //    {
        //        tos.Add(new EmailAddress(item));
        //    }

        //    var htmlStart = "<!DOCTYPE html>";
        //    var isHtml = message.StartsWith(htmlStart);

        //    SendGridMessage msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, isHtml ? "See Html." : message, message);


        //    // ReSharper disable once UnusedVariable
        //    var response = await client.SendEmailAsync(msg);
        //}

    }
}
