using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Nmpq.Tests {
	[TestFixture]
	public class FileTests {
		[Test]
		public void Open_returns_data_for_specified_file_if_it_exists_in_archive() {
			using(var archive = TestArchiveFactory.OpenTestArchive1()) {
				var listfile = archive.ReadFileBytes("(listfile)");

				Assert.That(listfile, Is.Not.Null);
			}
		}

		[Test]
		public void Listfile_contains_expected_data() {
			var expectedListfileContents = @"replay.attributes.events
replay.details
replay.game.events
replay.initData
replay.load.info
replay.message.events
replay.smartcam.events
replay.sync.events
";

			using(var archive = TestArchiveFactory.OpenTestArchive1()) {
				var listfile = archive.ReadFileBytes("(listfile)");
				Assume.That(listfile, Is.Not.Null.And.Not.Empty);

				var listfileContents = Encoding.ASCII.GetString(listfile);
				Assert.That(listfileContents, Is.EqualTo(expectedListfileContents));
			}
		}
	}
}