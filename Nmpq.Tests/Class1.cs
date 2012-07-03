using System.IO;
using NUnit.Framework;

namespace Nmpq.Tests {
	[TestFixture]
	public class HeaderParsingTests {
		private MpqReader Reader { 
			get { return new MpqReader(); }
		}

		[Test]
		public void No_exception_is_thrown_for_valid_mpq_archives() {
			var file = "TestArchives/Archive1.SC2Replay";

			Assume.That(File.Exists(file));
			Assert.DoesNotThrow(() => Reader.ReadArchive(file));
		}
	}
}
