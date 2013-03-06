using System.IO;
using System.Text;
using NUnit.Framework;

namespace Nmpq.Tests {
	[TestFixture]
	public class MpqFormatVersion3Tests {
		[Test]
		public void Can_parse_version_3_mpqs() {
			using (var archive = ObjectMother.OpenTestVersion3Mpq()) {
				Assert.That(archive, Is.Not.Null);
			}
		}

		[Test]
		public void Can_parse_FormatVersion() {
			using (var archive = ObjectMother.OpenTestVersion3Mpq()) {
				Assert.That(archive.Details.ArchiveHeader.FormatVersion, Is.EqualTo(3));
			}
		}

		[Test]
		public void Can_parse_header_size() {
			using (var archive = ObjectMother.OpenTestVersion3Mpq()) {
				Assert.That(archive.Details.ArchiveHeader.HeaderSize, Is.EqualTo(0xD0));
			}
		}

		[Test]
		public void Can_parse_listfile() {
			const string expected = @"replay.attributes.events
replay.details
replay.game.events
replay.initData
replay.load.info
replay.message.events
replay.smartcam.events
replay.sync.events
";

			using (var archive = ObjectMother.OpenTestVersion3Mpq()) {
				var listfile = Encoding.UTF8.GetString(archive.ReadFile("(listfile)"));

				Assert.That(listfile, Is.EqualTo(expected));
			}
		}

		[Test]
		[TestCase("replay.details")]
		[TestCase("replay.attributes.events")]
		[TestCase("replay.initData")]
		[TestCase("replay.message.events")]
		public void Can_parse_files(string path) {
			using (var archive = ObjectMother.OpenTestVersion3Mpq()) {
				var file = archive.ReadFile(path);
				var expected = File.ReadAllBytes(Path.Combine("TestArchives/V3/", path));

				Assert.That(file, Is.EqualTo(expected));
			}
		}

		[Test]
		public void Can_deserialize_data() {
			using (var archive = ObjectMother.OpenTestVersion3Mpq()) {
				var data = archive.ReadSerializedData("replay.details", true);

				Assert.That(data, Is.Not.Null);
			}
		}
	}
}