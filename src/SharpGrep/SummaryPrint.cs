using System;
using System.IO;

namespace SharpGrep
{
    /// <summary>
    /// Handes -c, -l, -L options,counts matches or lists files with/without matches.
    /// Processes stdin or files.
    /// </summary>
    public sealed class SummaryPrint
    {
        public int Run(Options options, RegexMatcher matcher)
        {
            // ---------- STDIN ----------
            if (options.Inputs.Length == 0)
            {
                int m = options.SearchStop;
                int count = CountMatchesFromReader(Console.In, matcher, m);
                // ----- -c ------
                if (options.CountOnly)
                {
                    Output.PrintCount(count, null, false);
                    return count > 0 ? ExitCodes.Success : ExitCodes.NoMatchesFound;
                }
                // ----- -l or -L ------
                else
                {
                    if (count > 0)
                    {
                        if (options.ListWithMatches)
                        {
                            Console.WriteLine("(standard input)");
                        }
                        return ExitCodes.Success;
                    }
                    else
                    {
                        if (options.ListWithoutMatches)
                        {
                            Console.WriteLine("(standard input)");
                        }
                        return ExitCodes.NoMatchesFound;
                    }
                }
            }

            // ---------- FILES ----------
            string[] files;
            bool multipleFiles;
            string err;
            if (!FileEnumerator.TryExpandInputs(options.Inputs, options.Recursive, out files, out multipleFiles, out err))
            {
                Console.WriteLine("Error opening file: " + err);
                return ExitCodes.FileError;
            }

            // ------ -c ------
            if (options.CountOnly)
            {
                bool anyNonZero = false;
                int m = options.SearchStop;

                foreach (var filePath in files)
                {
                    try
                    {
                        int c = CountMatchesFromFile(filePath, matcher, m);
                        Output.PrintCount(c, filePath, multipleFiles);
                        if (c > 0) anyNonZero = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error opening file: {ex.Message}");
                        return ExitCodes.FileError;
                    }
                }

                return anyNonZero ? ExitCodes.Success : ExitCodes.NoMatchesFound;
            }
            // ------ -l or -L ------
            else
            {
                bool any = false;
                foreach (var filePath in files)
                {
                    try
                    {
                        if (FileHasAnyMatch(filePath, matcher))
                        {
                            if (options.ListWithMatches)
                            {
                                Console.WriteLine(filePath);
                            }
                            any = true;
                        }
                        else
                        {
                            if (options.ListWithoutMatches)
                            {
                                Console.WriteLine(filePath);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error opening file: {ex.Message}");
                        return ExitCodes.FileError;
                    }
                }
                return any ? ExitCodes.Success : ExitCodes.NoMatchesFound;
            }
        }

        // Checks if the file has any match, returns true/false.
        private static bool FileHasAnyMatch(string path, RegexMatcher matcher)
        {
            if (BinaryDetector.IsProbablyBinary(path))
            {
                return matcher.IsBinaryFileMatch(path);
            }
            using (var reader = new StreamReader(path))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (matcher.IsMatch(line)) return true;
                }
                return false;
            }
        }

        // Counts matches in the file, up to limit m if m > 0.
        private static int CountMatchesFromFile(string path, RegexMatcher matcher, int m)
        {
            if (BinaryDetector.IsProbablyBinary(path))
            {
                return matcher.IsBinaryFileMatch(path) ? 1 : 0;
            }
            using (var reader = new StreamReader(path))
            {
                return CountMatchesFromReader(reader, matcher, m);
            }
        }

        // Counts matches from the TextReader, up to limit m if m > 0.
        private static int CountMatchesFromReader(TextReader reader, RegexMatcher matcher, int m)
        {
            int count = 0;
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (matcher.IsMatch(line))
                {
                    count++;
                    if (m > 0 && count >= m)
                    {
                        break;
                    }
                }
            }
            return count;
        }
    }
}
