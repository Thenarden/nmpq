using System;
using System.IO;
using System.Security.Cryptography;
using NUnit.Framework;

namespace Nmpq.Tests
{
    public static class ObjectMother
    {
        private static IMpqArchive OpenTestArchive(string path, string expectedHash)
        {
            Assume.That(File.Exists(path));

            var fileBytes = File.ReadAllBytes(path);

            using (var sha = SHA1.Create())
            {
                var hash = Convert.ToBase64String(sha.ComputeHash(fileBytes));
                Assume.That(hash, Is.EqualTo(expectedHash));
            }

            return MpqArchive.Open(path);
        }

        public static byte[] ReadTestFile(string path, string expectedHash)
        {
            Assume.That(File.Exists(path));

            var fileBytes = File.ReadAllBytes(path);

            using (var sha = SHA1.Create())
            {
                var hash = Convert.ToBase64String(sha.ComputeHash(fileBytes));
                Assume.That(hash, Is.EqualTo(expectedHash));
            }

            return fileBytes;
        }

        public static IMpqArchive OpenTestArchive1()
        {
            return OpenTestArchive("TestArchives/Archive1.SC2Replay", "2gMBq3cvcyaO2PLK7QWjuiigSQE=");
        }

        public static IMpqArchive OpenTestArchive2()
        {
            return OpenTestArchive("TestArchives/Archive2.s2ma", "CpPtnW7YaZHbKcsCXmIVwNv08BE=");
        }

        public static byte[] GetTestArchive2Minimap()
        {
            return ReadTestFile("TestArchives/Archive2-Minimap.tga", "/gD9VXczfO3PZsJjIC0eO7VbuTg=");
        }

        public static IMpqArchive OpenTestVersion3Mpq()
        {
            return OpenTestArchive("TestArchives/V3/MpqVersion3.SC2Replay", "FbiFU8Zf1Yb2eOmnma2/wkFnpEA=");
        }
    }
}