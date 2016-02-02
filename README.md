A collection of relatively small projects that don't deserve their own repository. This is my
collection of tools that I've built or found over the years to accomplish various tasks.

As a project outgrows its small-projects qualifications, it will be migrated into its own
repository. The link in this readme will move to the migrated section and the source will be
removed.

If a small project is unused for an extended period of time to the point of becoming outdated,
then it will be migrated to the retired section with its source zipped up.



## Table of Contents

Sorted alphabetically.

| Title                                         | Description                                       | Language | Date |
| :---                                          | :---                                              | :--- | :---: |
| [FileSystemEnumerable](#filesystemenumerable) | Friendlier enumerator for a file system.          | C# | 2015-03 |
| [GravatarGrab](#gravatargrab)                 | Retrieve a Gravatar by email.                     | Python 3 | 2015-03 |
| [HashRename](#hashrename)                     | Recursively renames files to their hash.          | C# | 2015-04 |
| [PerformanceTesting](#performancetesting)     | Perf test a function 10k times.                   | C# | 2015-08 |
| [Progress](#progress)                         | Time-based progress percentage.                   | C# | 2015-08 |
| [RandomPassword](#randompassword)             | Cryptographically secure alphanumeric passwords.  | C# | 2016-02 |
| [ReplaceAll](#replaceall)                     | Replace all with `std::string` objects.           | C++ | 2013-12 |
| [SendEmail](#sendemail)                       | Sends GMails with an auth file.                   | C# | 2015-05 |
| [Notes](#notes)                               | Things that need to be said.                      | N/A | - |



## FileSystemEnumerable

A C# class to enumerate every file/dir recursively with a search string that doesn't invalidate the
enumerable whilst traversing.

    var root = new DirectoryInfo(@"c:\wherever");
    var searchPattern = @"*.txt";
    var searchOption = SearchOption.AllDirectories;
    var enumerable = new FileSystemEnumerable(root, searchPattern, searchOption);
    
A la http://stackoverflow.com/a/13130054/4062072 on 2015-03-03.


    
## GravatarGrab

A Python script that when given an email address will attempt to retrieve the corresponding icon
from Gravatar and store it in a file.

    Usage: python3 GravatarGrab.py email "file name"

Useful bit of git to get the email and name from every commit in sorted order:

    git log --format='$aN|%aE' | sort -u



## HashRename

This C# tool will recursively hash and rename files.

    Usage: hashrename [OPTIONS] [FILES]+
    Hashes and renames files using the specified hash algorithm, recursively.
    Directories are left untouched, and this only prints files that were renamed.

    Options:
      -t, --type=VALUE           The type of hash algorithm to use for filenames.
                                   Curently supported is md5, sha1, sha256, and
                                   sha512. md5 is default.
      -d, --delete               If a duplicate hash is found, delete the file

    FILES:
            The list of files or directories to recursively hash and rename.



## PerformanceTesting

Edit `DoTheThing()` then compile and run to perform 10k perf tests on that function.



## Progress

Made to test the progress reading of ConEmu.

Specifically, this tool takes a number of seconds to progress through and then prints the current
percentage towards that goal on one line in the console. Shows every percent regardless of how
little time is set to elapse.



## RandomPassword

A cryptographically secure alphanumeric password generator. Takes one optional parameter for the 
length of the password, but defaults to 100 chars.



## ReplaceAll

A relatively efficient C++ function to perform a replace all on strings in C++ since there is no
built-in way to perform replace-all on `std::string` objects.



## SendEmail

A C# program that can be used to send emails with GMail via the use of a `.sendemail` auth file
located in the user's home directory (platform independent, of course).

    Usage: sendemail [OPTIONS]+
    Send an email using a JSON auth file stored in the home directory at .sendemail.

    This program only supports sending emails through GMail.

    Options:
      -h, --help                 Print this help text
          --generate-example     Generates an example auth file (.sendemail) in the
                                   home directory. This will overwrite an existing
                                   auth file
      -s, --subject=VALUE        Email subject line. If this is omitted, it will be
                                   the name of the file specified for the body
                                   option.
      -b, --body=VALUE           The plaintext file containing the body of the email

      -t, --to=VALUE             To email address
          --cc=VALUE             CC email address
          --bcc=VALUE            BCC email address
      -a, --attachment=VALUE     Adds an attachment to the email



## Notes

Things that don't really go anywhere else, but should go somewhere.

#### C# on Linux

If a small project is in C# using Visual Studio and you want to compile on Linux:

    apt-get install mono-complete
    xbuild csharpproject.csproj
    
If a small project is just a C# file and you want to compile on Linux:

    apt-get install mono-complete
    mcs csharpfile.cs

If a small project has a DLL associated with it and you wish to package it into one executable,
while also removing all extraneous unused code from the DLLs:

    mkbundle -o <outfile> <exefile> <dllfile1> <dllfile2> ...

#### Mono Trust Issues

A la http://stackoverflow.com/a/9803922/4062072, a default installation of Mono does not trust
anyone! This makes it very hard to use any sort of certificate authorities (such as for the
sending email scripts).

To install all certificates in the Mozilla store (most user cases, but technically the least safe),
in addition to the specific gmail certificate:

    mozroots --import --ask-removed
    certmgr -ssl smtps://smtp.gmail.com:465
