using System;
using System.Collections.Generic;
using System.IO;

namespace SharpGrep
{
    public static class FileEnumerator
    {
        public static bool TryExpandInputs(
            string[] inputs,
            bool recursive,
            out string[] files,
            out bool multipleFiles,
            out string errorMessage)
        {
            var list = new List<string>();
            errorMessage = "";

            for (int i = 0; i < inputs.Length; i++)
            {
                string path = inputs[i];

                try
                {
                    if (File.Exists(path))
                    {
                        list.Add(path);
                    }
                    else if (Directory.Exists(path))
                    {
                        if (!recursive)
                        {
                            errorMessage = $"'{path}' is a directory (use -r to search recursively).";
                            files = Array.Empty<string>();
                            multipleFiles = false;
                            return false;
                        }

                        // Collect all files under the directory recursively.
                        string[] inner = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                        for (int j = 0; j < inner.Length; j++)
                        {
                            list.Add(inner[j]);
                        }
                    }
                    else
                    {
                        errorMessage = $"File or directory not found: {path}";
                        files = Array.Empty<string>();
                        multipleFiles = false;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    files = Array.Empty<string>();
                    multipleFiles = false;
                    return false;
                }
            }

            files = list.ToArray();
            multipleFiles = files.Length > 1;
            return true;
        }
    }
}
