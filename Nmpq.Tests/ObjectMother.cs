using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using NUnit.Framework;

namespace Nmpq.Tests {
	public static class ObjectMother {
		private static IMpqArchive OpenTestArchive(string path, string expectedHash) {
			Assume.That(File.Exists(path));

			var fileBytes = File.ReadAllBytes(path);

			using (var sha = SHA1.Create()) {
				var hash = Convert.ToBase64String(sha.ComputeHash(fileBytes));
				Assume.That(hash, Is.EqualTo(expectedHash));
			}

			return MpqArchive.Open(path);
		}

		public static byte[] ReadTestFile(string path, string expectedHash) {
			Assume.That(File.Exists(path));

			var fileBytes = File.ReadAllBytes(path);

			using (var sha = SHA1.Create()) {
				var hash = Convert.ToBase64String(sha.ComputeHash(fileBytes));
				Assume.That(hash, Is.EqualTo(expectedHash));
			}

			return fileBytes;
		}

		public static IMpqArchive OpenTestArchive1() {
			return OpenTestArchive("TestArchives/Archive1.SC2Replay", "2gMBq3cvcyaO2PLK7QWjuiigSQE=");
		}

		public static IMpqArchive OpenTestArchive2() {
			return OpenTestArchive("TestArchives/Archive2.s2ma", "CpPtnW7YaZHbKcsCXmIVwNv08BE=");
		}

        public static IMpqArchive OpenTestArchive(string filename)
        {
            const string dir = "TestArchives";

            var path = Path.Combine(dir, filename);
            var expectedHash = Hashes[path];
            return OpenTestArchive(path, expectedHash);
        }

        private static readonly IDictionary<string, string> Hashes = new Dictionary<string, string> (StringComparer.OrdinalIgnoreCase)
                                                                         {
                                                                             { "TestArchives\\Archive1.SC2Replay",  "2gMBq3cvcyaO2PLK7QWjuiigSQE=" },
                                                                             { "TestArchives\\Archive2.s2ma",       "CpPtnW7YaZHbKcsCXmIVwNv08BE=" },
                                                                             { "TestArchives\\Archive3.SC2Replay",  "/QMroQqb+v3kIpqeaDaIr1hbJIQ=" },
                                                                         };

	    public static ICollection<TestCaseData> AllArchives
	    {
	        get
	        {
                return new[]
                           {
                               new TestCaseData("Archive1.SC2Replay"), 
                               new TestCaseData("Archive2.s2ma"), 
                               new TestCaseData("Archive3.SC2Replay")
                           };
	        }
	    }

		public static byte[] GetTestArchive2Minimap() {
			return ReadTestFile("TestArchives/Archive2-Minimap.tga", "/gD9VXczfO3PZsJjIC0eO7VbuTg=");
		}

		public static IMpqArchive OpenTestVersion3Mpq() {
			return OpenTestArchive("TestArchives/V3/MpqVersion3.SC2Replay", "FbiFU8Zf1Yb2eOmnma2/wkFnpEA=");
		}
	}
}