using System.IO;
using Moq;
using NUnit.Framework;

namespace Nmpq.Tests {
	[TestFixture]
	public class HeaderParsingTests {
		[Test]
		public void No_exception_is_thrown_for_valid_mpq_archives() {
			var path = "TestArchives/Archive1.SC2Replay";

			Assume.That(File.Exists(path));
			Assert.DoesNotThrow(() => MpqArchive.Open(path));
		}
	}

	[TestFixture]
	public class CleanupTests {
		[Test]
		public void Passed_stream_is_not_disposed_when_archive_is_disposed() {
			var path = "TestArchives/Archive1.SC2Replay";
			
			Assume.That(File.Exists(path));

			using(var file = File.OpenRead(path)) {
				var stream = new TestingStreamWrapper(file);
				using(MpqArchive.Open(stream)) {}

				Assert.That(stream.IsDisposed, Is.False);
			}
		}
	}
}
