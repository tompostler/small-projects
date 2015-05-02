using Mono.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace sendemail
{
    class ParseOptions
    {
        public static void Parse(string[] args, out string subject, out FileInfo body,
            out Dictionary<string, MailAddressCollection> addresses, out AuthFile from,
            out List<FileInfo> attachments)
        {
            bool help = false;
            bool sampleauth = false;

            string lsubject = null;
            string lbody = null;

            // Options
            OptionSet p = new OptionSet()
            {
                "",
                "Usage: sendemail [OPTIONS]+",
                "Send an email using a JSON auth file stored in the home directory at .sendemail.",
                "",
                "This program only supports sending emails through GMail",
                "",
                "A default installation of Mono doesn't trust anyone. You may need to run the " +
                "following commands in order to get Mono to send emails:",
                "    mozroots --import --ask-remove",
                "    certmgr -ssl smtps://smtp.gmail.com:465",
                "",
                "Options:",
                {
                    "h|help",
                    "Print this help text",
                    _ => help = true
                },
                {
                    "generate-example",
                    "Generates an example auth file (.sendemail) in the home directory. This will overwrite an existing auth file",
                    _ => sampleauth = true
                },
                {
                    "s|subject=",
                    "Email subject line. If this is omitted, it will be the name of the file specified for the body option.",
                    opt => lsubject = opt
                },
                {
                    "b|body=",
                    "The plaintext file containing the body of the email",
                    opt => lbody = opt
                }
            };

            // Email addresses
            List<string> toAddresses = new List<string>();
            p.Add("t|to=", "To email address", opt =>
            {
                toAddresses.Add(opt);
            });
            List<string> ccAddresses = new List<string>();
            p.Add("cc=", "CC email address", opt =>
            {
                ccAddresses.Add(opt);
            });
            List<string> bccAddresses = new List<string>();
            p.Add("bcc=", "BCC email address", opt =>
            {
                bccAddresses.Add(opt);
            });

            // Attachments
            List<string> lattachments = new List<string>();
            p.Add("a|attachment=", "Adds an attachment to the email", opt =>
            {
                lattachments.Add(opt);
            });

            // Parse options
            try
            {
                p.Parse(args);
            }
            catch (OptionException e)
            {
                Util.fail(p, e.Message);
            }

            // Check for help text
            if (help)
                Util.fail(p);

            // Check for sample/regular auth
            string authPath = Path.Combine(Util.HomeDirectoryPath(), ".sendemail");
            if (sampleauth)
                GenerateSampleAuth(authPath);
            if (!File.Exists(authPath))
                Util.fail(p, "Auth file required");
            from = (AuthFile)JsonConvert.DeserializeObject(File.ReadAllText(authPath), typeof(AuthFile));

            // Check auth email validity
            if (!IsValidEmail(from.from))
                Util.fail(msg: "Auth file needs valid from email");

            // Check email validity
            addresses = new Dictionary<string, MailAddressCollection>()
            {
                { "to", new MailAddressCollection()},
                { "cc", new MailAddressCollection()},
                { "bcc", new MailAddressCollection()}
            };
            foreach (string email in toAddresses)
                if (IsValidEmail(email))
                    addresses["to"].Add(new MailAddress(email));
                else
                    Util.fail(msg: "Invalid 'to' email (" + email + ")");
            foreach (string email in ccAddresses)
                if (IsValidEmail(email))
                    addresses["cc"].Add(new MailAddress(email));
                else
                    Util.fail(msg: "Invalid 'cc' email (" + email + ")");
            foreach (string email in bccAddresses)
                if (IsValidEmail(email))
                    addresses["bcc"].Add(new MailAddress(email));
                else
                    Util.fail(msg: "Invalid 'bcc' email (" + email + ")");
            if (addresses["to"].Count == 0)
                Util.fail(p, "There must be at least one 'to' address");

            // Body
            body = null;
            if (!String.IsNullOrEmpty(lbody))
                if (!File.Exists(lbody))
                    Util.fail(p, "Body file does not exist");
                else
                    body = new FileInfo(lbody);

            // Subject
            subject = lsubject;

            // Check for valid attachments
            attachments = new List<FileInfo>();
            foreach (string attachment in lattachments)
                if (!File.Exists(attachment))
                    Util.fail(p, "Attachment problem (" + attachment + ")");
                else
                    attachments.Add(new FileInfo(attachment));
        }

        private static void GenerateSampleAuth(string authPath)
        {
            string json = JsonConvert.SerializeObject(new AuthFile
            {
                from = "Send Email <sendemail@example.com>",
                pass = "MySuperSecretPa$$word"
            }, Formatting.Indented);

            File.WriteAllText(authPath, json);

            Util.fail(msg: "Sample auth file generated at " + authPath);
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                MailAddress address = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
