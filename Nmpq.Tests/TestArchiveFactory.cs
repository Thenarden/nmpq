using System;
using System.IO;
using System.Security.Cryptography;
using NUnit.Framework;

namespace Nmpq.Tests {
	public static class TestArchiveFactory {
		public static MpqArchive OpenTestArchive1() {
			var path = "TestArchives/Archive1.SC2Replay";
			var expectedHash = "2gMBq3cvcyaO2PLK7QWjuiigSQE=";

			Assume.That(File.Exists(path));

			var fileBytes = File.ReadAllBytes(path);

			using(var sha = SHA1.Create()) {
				var hash = Convert.ToBase64String(sha.ComputeHash(fileBytes));
				Assume.That(hash, Is.EqualTo(expectedHash));
			}

			return MpqArchive.Open(path);
		}
	}
}