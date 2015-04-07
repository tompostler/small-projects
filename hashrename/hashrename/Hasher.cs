using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace hasher
{
    class Hasher
    {
        private HashAlgorithm hashAlgorithm;
        public bool dup { get; set; }

        private SortedList<string, string> renamed;

        /// <summary>
        /// Ctor.
        /// </summary>
        public Hasher(HashAlgorithm hashAlgorithm)
        {
            this.hashAlgorithm = hashAlgorithm;

            renamed = new SortedList<string, string>();
        }

        public void HashRename(List<string> thingsToHash)
        {
            hashRenameThingsFromEnumerable(thingsToHash);
        }

        /// <summary>
        /// Hash all the files, or files in dirs recursively, in the enumerable. Output the renamed files to stdout.
        /// </summary>
        private void hashRenameThingsFromEnumerable(IEnumerable<string> pathsToHashRename)
        {
            List<IEnumerable<string>> pathsToEnumerate = getEnumerablePathsFromPaths(pathsToHashRename);

            hashRenameTheThings(pathsToEnumerate);
        }

        private static List<IEnumerable<string>> getEnumerablePathsFromPaths(IEnumerable<string> paths)
        {
            List<IEnumerable<string>> pathsToEnumerate = new List<IEnumerable<string>>();
            List<string> pathFiles = new List<string>();

            // For every input, add it to the list of directories to enumerate or the list of files
            foreach (string path in paths)
            {
                string fullPath = Path.GetFullPath(path);
                FileAttributes attr = File.GetAttributes(fullPath);

                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    pathsToEnumerate.Add(new FileSystemEnumerable(path, "*", SearchOption.AllDirectories));
                else
                    pathFiles.Add(fullPath);
            }

            // Add the individual files to the pathsToEnumerate as a pseudo-directory
            pathsToEnumerate.Add(pathFiles);
            return pathsToEnumerate;
        }

        /// <summary>
        /// Hashes and renames all the the things in the enumerables in the list.
        /// </summary>
        /// <param name="filesToEnumerateAndHash"></param>
        private void hashRenameTheThings(List<IEnumerable<string>> filesToEnumerateAndHash)
        {
            foreach (IEnumerable<string> filesToHash in filesToEnumerateAndHash)
            {
                foreach (string fileToHash in filesToHash)
                {
                    try
                    {
                        Tuple<string, string> oldAndNew = hashRenameAFile(fileToHash);
                        if (oldAndNew != null)
                        {
                            string line = String.Format("{0} <- {1}", oldAndNew.Item2, oldAndNew.Item1);
                            Console.WriteLine(line);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                    catch (IOException e)
                    {
                        Console.Error.WriteLine('\n' + e.Message.Replace("\n", ""));
                        Console.Error.WriteLine("NAME: " + fileToHash);
                        Console.Error.WriteLine("HASH: " + hashAFile(fileToHash) + '\n');
                        if (!this.dup)
                            Environment.Exit(Environment.ExitCode);
                    }
                }
            }
        }

        /// <summary>
        /// Hashes one file.
        /// </summary>
        /// <param name="fileToHashRename">The path to the file to hash.</param>
        /// <returns>A tuple of the old filename and the new filename.</returns>
        private Tuple<string, string> hashRenameAFile(string fileToHashRename)
        {
            // Current name and hash
            FileInfo currentFile = new FileInfo(fileToHashRename);
            string oldname = currentFile.Name;
            string hash = hashAFile(fileToHashRename);

            // Name and hash are equal
            if (oldname == hash + currentFile.Extension)
                return null;

            // Rename or delete/exit
            try
            {
                currentFile.MoveTo(currentFile.Directory.FullName + "\\" + hash + currentFile.Extension);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine('\n' + e.Message.Replace("\n", ""));
                Console.Error.WriteLine("NAME: " + fileToHashRename);
                Console.Error.WriteLine("HASH: " + hash + '\n');
                if (this.dup)
                {
                    currentFile.Delete();
                    return null;
                }
                else
                    Environment.Exit(Environment.ExitCode);
            }

            // Return
            string newname = currentFile.Name;
            return new Tuple<string, string>(oldname, newname);
        }

        private string hashAFile(string fileToHash)
        {
            using (var stream = File.OpenRead(fileToHash))
            {
                return BitConverter.ToString(hashAlgorithm.ComputeHash(stream)).Replace("-", "").ToLower();
            }
        }
    }
}
