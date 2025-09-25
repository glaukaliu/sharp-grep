using System;
using System.IO;
using System.Collections.Generic;

namespace SharpGrep
{
    /// <summary>
    /// Handles streaming text input (stdin or files), applies the regex matcher,
    /// manages context lines, and prints matching lines or only-matches as specified.
    /// </summary>
    public sealed class NormalPrint
    {
        private static bool ShouldColorize(ColorMode mode)
        {
            if (mode == ColorMode.Always) return true;
            if (mode == ColorMode.Never)  return false;
            // Auto mode:
            return !Console.IsOutputRedirected;
        }

        public int Run(Options options, RegexMatcher matcher)
        {
            bool matchesPattern = false;

            if (options.Inputs.Length == 0)
            {
                //  ---------- STANDARD INPUT ----------
                string? line;

                Context beforeContext = new Context(options.Before);
                int remainingAfter = 0;

                int m = options.SearchStop;
                int matchCount = 0;

                bool colorOn = ShouldColorize(options.Color);


                while ((line = Console.In.ReadLine()) != null)
                {
                    // ------ normal line output ------
                    if (!options.OnlyMatches)
                    {
                        if (matcher.IsMatch(line))
                        {
                            matchCount++;
                            if (m > 0 && matchCount > m)
                            {
                                break;
                            }
                            Output.PrintBeforeContext(beforeContext, null, false);
                            if (colorOn)
                            {
                                var spans = new List<(int start, int length)>();
                                matcher.CollectMatches(line, spans);
                                string colored = Output.BuildColoredLine(line, spans);
                                Console.WriteLine(colored);
                            }
                            else
                            {
                                Console.WriteLine(line);
                            }
                            matchesPattern = true;
                            remainingAfter = options.After;
                        }
                        else
                        {
                            Output.HandleAfterContext(line, null, false, ref remainingAfter, beforeContext, options.Before);
                        }
                    }

                    // ------ -o ------	
                    else
                    {
                        // Collect all matches in the line
                        var matches = new List<(int start, int length)>();
                        matcher.CollectMatches(line, matches);
                        if (matches.Count > 0)
                        {
                            matchCount++;
                            // checks if stop limit -m has reached
                            if (m > 0 && matchCount > m)
                            {
                                break;
                            }
                            Output.PrintBeforeContext(beforeContext, null, false);
                            if (colorOn)
                                Output.PrintOnlyMatchesColored(line, matches, null, false);
                            else
                            {
                                Output.PrintOnlyMatches(line, matches, null, false);
                            }
                            matchesPattern = true;
                            remainingAfter = options.After;
                        }
                        else
                        {
                            Output.HandleAfterContext(line, null, false, ref remainingAfter, beforeContext, options.Before);
                        }

                    }
                }
            }
            else
            {
                //  ---------- FILES ----------
                string[] files;
                bool multipleFiles;
                string errorMessage;
                // Expand input paths into file list, recursively if -r
                if (!FileEnumerator.TryExpandInputs(options.Inputs, options.Recursive, out files, out multipleFiles, out errorMessage))
                {
                    Console.WriteLine($"Error: {errorMessage}");
                    return ExitCodes.ArgumentError;
                }

                bool colorOn = ShouldColorize(options.Color);

                // Process each file
                foreach (var filePath in files)
                {
                    try
                    {
                        // Check for binary file
                        if (BinaryDetector.IsProbablyBinary(filePath))
                        {
                            bool bmatch = matcher.IsBinaryFileMatch(filePath);
                            if (bmatch)
                            {
                                Output.PrintBinaryFileMatch(filePath);
                                matchesPattern = true;
                            }
                            // Regardless of match or not, skip text streaming of this file
                            continue;
                        }
                        using (StreamReader reader = new StreamReader(filePath))
                        {
                            string? line;

                            Context beforeContext = new Context(options.Before);
                            int remainingAfter = 0;
                            int m = options.SearchStop;
                            int matchCount = 0;

                            while ((line = reader.ReadLine()) != null)
                            {
                                if (!options.OnlyMatches)
                                {
                                    if (matcher.IsMatch(line))
                                    {
                                        matchCount++;
                                        if (m > 0 && matchCount > m)
                                        {
                                            break;
                                        }
                                        Output.PrintBeforeContext(beforeContext, filePath, multipleFiles);
                                        if (colorOn)
                                        {
                                            var spans = new List<(int start, int length)>();
                                            matcher.CollectMatches(line, spans);
                                            string colored = Output.BuildColoredLine(line, spans);
                                            Output.PrintLine(colored, filePath, multipleFiles);
                                        }
                                        else
                                        {
                                            Output.PrintLine(line, filePath, multipleFiles);
                                        }
                                        matchesPattern = true;

                                        remainingAfter = options.After;
                                    }
                                    else
                                    {
                                        Output.HandleAfterContext(line, filePath, multipleFiles, ref remainingAfter, beforeContext, options.Before);
                                    }
                                }

                                // ------ -o ------	
                                else
                                {
                                    var matches = new List<(int start, int length)>();
                                    matcher.CollectMatches(line, matches);
                                    if (matches.Count > 0)
                                    {
                                        matchCount++;
                                        if (m > 0 && matchCount > m)
                                        {
                                            break;
                                        }
                                        Output.PrintBeforeContext(beforeContext, filePath, multipleFiles);
                                        if (colorOn)
                                            Output.PrintOnlyMatchesColored(line, matches, filePath, multipleFiles);
                                        else
                                        {
                                            Output.PrintOnlyMatches(line, matches, filePath, multipleFiles);
                                        }   
                                        matchesPattern = true;
                                        remainingAfter = options.After;
                                    }
                                    else
                                    {
                                        Output.HandleAfterContext(line, filePath, multipleFiles, ref remainingAfter, beforeContext, options.Before);
                                    }
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error opening file: {ex.Message}");
                        return ExitCodes.FileError;
                    }
                }
            }
            return matchesPattern ? ExitCodes.Success : ExitCodes.NoMatchesFound;

        }
    }
}
