using System.IO;
using NUnit.Framework;

namespace Nmpq.Tests {
	[TestFixture]
	public class FileTests {
		[Test]
		public void Open_returns_data_for_specified_file_if_it_exists_in_archive() {
			var path = "TestArchives/Archive1.SC2Replay";
			Assume.That(File.Exists(path));

			using(var archive = MpqArchive.Open(path)) {
				var listfile = archive.ReadFileBytes("(listfile)");

				Assert.That(listfile, Is.Not.Null);
			}
		}
	}
}