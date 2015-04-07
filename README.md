A collection of relatively small projects that don't deserve their own repository. This is my
collection of tools that I've built or found over the years to accomplish various tasks.

As a project outgrows its small-projects qualifications, it will be migrated into its own
repository. The link in this readme will move to the migrated section and the source will be
removed.

If a small project is unused for an extended period of time to the point of becoming outdated,
then it will be migrated to the retired section with its source zipped up.



## Table of Contents

Sorted alphabetically.

| Title | Description | Language | Date |
| :--- | :--- | :--- | :---: |
| [GravatarGrab](#GravatarGrab) | Retrieve a Gravatar image by email. | Python 3 | 2015-03 |
| [HashRename](#HashRename) | Recursively renames files to their hash. | C# | 2015-04 |
| [ReplaceAll](#ReplaceAll) | A function to perform a replace all with `std::string`s. | C++ | 2013-12 |
| [Notes](#Notes) | Things that need to be said. | N/A | - |


    
## GravatarGrab

A Python script that when given an email address will attempt to retrieve the corresponding icon
from Gravatar and store it in a file.

    Usage: python3 GravatarGrab.py email "file name"



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

            
            
## ReplaceAll

A relatively efficient C++ function to perform a replace all on strings in C++ since there is no
built-in way to perform replace-all on `std::string` objects.



## Notes

Things that don't really go anywhere else, but should go somewhere.

#### C# on Linux

If a small project is in C# using Visual Studio and you want to compile on Linux:

    apt-get install mono-complete
    xbuild csharpproject.csproj
    
If a small project is just a C# file and you want to compile on Linux:

    apt-get install mono-complete
    mcs csharpfile.cs
