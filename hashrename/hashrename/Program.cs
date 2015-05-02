using hasher;
using Mono.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace hashrename
{
    class Program
    {
        static void Main(string[] args)
        {
            bool dup = false;
            string alg = null;

            // Options
            OptionSet p = new OptionSet
            {
                "",
                "Usage: hashrename [OPTIONS] [FILES]+",
                "Hashes and renames files using the specified hash algorithm, recursively. Directories are left untouched, and this only prints files that were renamed.",
                "",
                "Options:",
                {
                    "t|type=",
                    "The type of hash algorithm to use for filenames. Curently supported is crc32, md5, sha1, sha256, and sha512. md5 is default.",
                    v => alg = v
                },
                {
                    "d|delete",
                    "If a duplicate hash is found, delete the file",
                    v => dup = (v != null)
                },
                "",
                "FILES:",
                "\tThe list of files or directories to recursively hash and rename."
            };

            // Get the files
            List<string> files;
            try
            {
                files = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("HASHRENAME: ");
                Console.WriteLine(e.Message);
                return;
            }

            // Handle problem
            if (files.Count == 0)
            {
                p.WriteOptionDescriptions(Console.Out);
                Environment.Exit(Environment.ExitCode);
            }

            // Do stuff
            Hasher hasher = null;
            if (String.IsNullOrEmpty(alg) || alg == "md5")
                hasher = new Hasher(MD5.Create());
            else if (alg == "crc32")
                hasher = new Hasher(Crc32.Create());
            else if (alg == "sha1")
                hasher = new Hasher(SHA1.Create());
            else if (alg == "sha256")
                hasher = new Hasher(SHA256.Create());
            else if (alg == "sha512")
                hasher = new Hasher(SHA512.Create());
            else
            {
                p.WriteOptionDescriptions(Console.Out);
                Environment.Exit(Environment.ExitCode);
            }
            hasher.dup = dup;
            hasher.HashRename(files);
        }
    }
}
