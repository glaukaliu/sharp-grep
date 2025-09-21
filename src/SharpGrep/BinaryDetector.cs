using System;
using System.IO;

namespace SharpGrep
{
    public static class BinaryDetector
    {
        private const int SampleSize = 8192;
        private const double ControlMaxRatio = 0.01;  // >2% controls => binary

        // True = probably binary, False = looks like text
        public static bool IsProbablyBinary(string filePath)
        {
            try
            {
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                int toRead = (int)Math.Min(SampleSize, fs.Length);
                if (toRead <= 0) return false; //

                byte[] buf = new byte[toRead];
                int read = fs.Read(buf, 0, toRead);
                if (read <= 0) return false;

                // NUL byte => binary
                for (int i = 0; i < read; i++)
                    if (buf[i] == 0x00) return true;

                // Count ASCII control chars excluding \t(09), \n(0A), \r(0D)
                int controls = 0;
                for (int i = 0; i < read; i++)
                {
                    byte b = buf[i];
                    if (b == 0x09 || b == 0x0A || b == 0x0D) continue;
                    if (b < 0x20 || b == 0x7F) controls++;
                }

                double ratio = (double)controls / read;
                return ratio > ControlMaxRatio;
            }
            catch
            {
                // On error, treat as text.
                return false;
            }
        }
    }
}
