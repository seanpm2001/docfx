// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DocAsCode.Glob;

public class FileGlob
{
    public static IEnumerable<string> GetFiles(string cwd, IEnumerable<string> patterns, IEnumerable<string> excludePatterns, GlobMatcherOptions options = GlobMatcher.DefaultOptions)
    {
        if (patterns == null)
        {
            return Enumerable.Empty<string>();
        }

        if (string.IsNullOrEmpty(cwd))
        {
            cwd = Directory.GetCurrentDirectory();
        }
        var globArray = patterns.Select(s => new GlobMatcher(s, options)).ToArray();
        var excludeGlobArray = excludePatterns == null ?
            new GlobMatcher[0] :
            excludePatterns.Select(s => new GlobMatcher(s, options)).ToArray();
        return GetFilesCore(cwd, globArray, excludeGlobArray);
    }

    private static IEnumerable<string> GetFilesCore(string cwd, GlobMatcher[] globs, GlobMatcher[] excludeGlobs)
    {
        if (!Directory.Exists(cwd))
        {
            yield break;
        }
        foreach (var file in GetFilesFromSubfolder(cwd, cwd, globs, excludeGlobs))
        {
            yield return NormalizeToFullPath(file);
        }
    }

    private static IEnumerable<string> GetFilesFromSubfolder(string baseDirectory, string cwd, GlobMatcher[] globs, GlobMatcher[] excludeGlobs)
    {
        foreach (var file in Directory.GetFiles(baseDirectory, "*", SearchOption.TopDirectoryOnly))
        {
            var relativePath = GetRelativeFilePath(cwd, file);
            if (IsFileMatch(relativePath, globs, excludeGlobs))
            {
                yield return file;
            }
        }

        foreach (var dir in Directory.GetDirectories(baseDirectory, "*", SearchOption.TopDirectoryOnly))
        {
            var relativePath = GetRelativeDirectoryPath(cwd, dir);
            if (IsDirectoryMatch(relativePath, globs, excludeGlobs))
            {
                foreach (var file in GetFilesFromSubfolder(dir, cwd, globs, excludeGlobs))
                {
                    yield return file;
                }
            }
        }
    }

    private static string GetRelativeFilePath(string directory, string file)
    {
        var subpath = file.Substring(directory.Length);
        // directory could be
        // 1. root folder, e.g. E:\ or /
        // 2. sub folder, e.g. a or a/ or a\
        return subpath.TrimStart('\\', '/');
    }

    private static string GetRelativeDirectoryPath(string parentDirectory, string directory)
    {
        var relativeDirectory = GetRelativeFilePath(parentDirectory, directory);
        return relativeDirectory.TrimEnd('\\', '/') + "/";
    }

    private static string NormalizeToFullPath(string path)
    {
        return Path.GetFullPath(path).Replace('\\', '/');
    }

    private static bool IsFileMatch(string path, GlobMatcher[] globs, GlobMatcher[] excludeGlobs)
    {
        return IsMatch(path, globs, excludeGlobs, false);
    }

    private static bool IsDirectoryMatch(string path, GlobMatcher[] globs, GlobMatcher[] excludeGlobs)
    {
        return IsMatch(path, globs, excludeGlobs, true);
    }

    private static bool IsMatch(string path, GlobMatcher[] globs, GlobMatcher[] excludeGlobs, bool partial)
    {
        foreach (var exclude in excludeGlobs)
        {
            if (exclude.Match(path, false)) return false;
        }
        foreach (var glob in globs)
        {
            if (glob.Match(path, partial)) return true;
        }
        return false;
    }
}