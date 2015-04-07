using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// A way to get enumerables for the filesystem in a Directory.EnumerateFiles fashion.
/// </summary>
/// <remarks>
/// a la http://stackoverflow.com/a/13130054 on 2015-03-03
/// </remarks>
public class FileSystemEnumerable : IEnumerable<string>
{
    private readonly string _root;
    private readonly IList<string> _patterns;
    private readonly SearchOption _option;

    public FileSystemEnumerable(string root, string pattern, SearchOption option)
    {
        _root = root;
        _patterns = new List<string> { pattern };
        _option = option;
    }

    public FileSystemEnumerable(string root, IList<string> patterns, SearchOption option)
    {
        _root = root;
        _patterns = patterns;
        _option = option;
    }

    public IEnumerator<string> GetEnumerator()
    {
        if (_root == null || !Directory.Exists(_root)) yield break;

        IEnumerable<string> matches = new List<string>();
        try
        {
            foreach (string pattern in _patterns)
            {
                matches = matches.Concat(Directory.EnumerateDirectories(_root, pattern, SearchOption.TopDirectoryOnly))
                                 .Concat(Directory.EnumerateFiles(_root, pattern, SearchOption.TopDirectoryOnly));
            }
        }
        catch (UnauthorizedAccessException)
        {
            yield break;
        }
        catch (PathTooLongException)
        {
            yield break;
        }

        foreach (var file in matches)
        {
            yield return file;
        }

        if (_option == SearchOption.AllDirectories)
        {
            foreach (var dir in Directory.EnumerateDirectories(_root, "*", SearchOption.TopDirectoryOnly))
            {
                var fileSystemInfos = new FileSystemEnumerable(dir, _patterns, _option);
                foreach (var match in fileSystemInfos)
                {
                    yield return match;
                }
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
