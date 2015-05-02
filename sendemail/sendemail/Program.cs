using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace sendemail
{
    class Program
    {
        static void Main(string[] args)
        {
            // Options
            string subject;
            FileInfo body;
            Dictionary<string, MailAddressCollection> addresses;
            List<FileInfo> attachments;
            AuthFile from;
            ParseOptions.Parse(args, out subject, out body, out addresses, out from, out attachments);

            // Set up the from address
            MailAddress fromAddr = new MailAddress(from.from);

            // Set up the message
            MailMessage message = new MailMessage();

            // from
            message.From = fromAddr;

            // to
            foreach (MailAddress addr in addresses["to"])
                message.To.Add(addr);

            // cc
            foreach (MailAddress addr in addresses["cc"])
                message.CC.Add(addr);

            // bcc
            foreach (MailAddress addr in addresses["bcc"])
                message.Bcc.Add(addr);

            // subject
            if (!String.IsNullOrEmpty(subject))
                message.Subject = subject;

            // body
            if (body != null)
                message.Body = File.ReadAllText(body.FullName);

            // attachments
            foreach (FileInfo attachment in attachments)
                message.Attachments.Add(new Attachment(attachment.FullName));

            // Set up the connection
            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddr.Address, from.pass)
            };

            smtp.Send(message);
        }
    }
}
