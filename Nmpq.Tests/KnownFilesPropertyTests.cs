using NUnit.Framework;

namespace Nmpq.Tests {
	[TestFixture]
	public class KnownFilesPropertyTests {
		[Test]
		public void KnownFiles_contains_expected_entries_for_test_archive() {
			using (var archive = TestArchiveFactory.OpenTestArchive1()) {
				Assert.That(archive.KnownFiles, Contains.Item("replay.details"));
				Assert.That(archive.KnownFiles, Contains.Item("replay.initData"));
				Assert.That(archive.KnownFiles, Contains.Item("replay.load.info"));
				Assert.That(archive.KnownFiles, Contains.Item("replay.game.events"));
				Assert.That(archive.KnownFiles, Contains.Item("replay.sync.events"));
				Assert.That(archive.KnownFiles, Contains.Item("replay.message.events"));
				Assert.That(archive.KnownFiles, Contains.Item("replay.smartcam.events"));
				Assert.That(archive.KnownFiles, Contains.Item("replay.attributes.events"));
			}
		}
	}
}