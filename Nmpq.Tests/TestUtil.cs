using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Nmpq.Tests
{
    public static class TestUtil
    {
        public static byte[] LoadBytesFromTextFile(string path)
        {
            Assume.That(File.Exists(path));

            return ParseBytes(File.ReadAllText(path));
        }

        public static byte[] ParseBytes(string byteList)
        {
            if (byteList == null) throw new ArgumentNullException("byteList");

            return byteList
                .Split(new[] {' ', '\r', '\n', '\t'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Convert.ToByte(s, 16))
                .ToArray();
        }
    }
}