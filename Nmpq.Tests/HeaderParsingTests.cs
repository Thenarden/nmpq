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
}
