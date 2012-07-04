using System.IO;
using NUnit.Framework;

namespace Nmpq.Tests {
	[TestFixture]
	public class HeaderTests {
		private MpqArchive OpenTestArchive() {
			var path = "TestArchives/Archive1.SC2Replay";
			Assume.That(File.Exists(path));
			return MpqArchive.Open(path);
		}

		[Test]
		public void Magic_is_read_as_expected() {
			using(var archive = OpenTestArchive()) {
				var magic = archive.Header.Magic;

				Assert.That(magic, Is.EqualTo(0x1a51504d));
			}
		}

		[Test]
		public void Header_size_is_read_as_expected() {
			using(var archive = OpenTestArchive()) {
				Assert.That(archive.Header.HeaderSize, Is.EqualTo(0x2c));
			}
		}

		[Test]
		public void Archive_size_is_read_as_expected() {
			using (var archive = OpenTestArchive()) {
				Assert.That(archive.Header.ArchiveSize, Is.EqualTo(83244));
			}
		}

		[Test]
		public void Format_is_read_as_expected() {
			using (var archive = OpenTestArchive()) {
				Assert.That(archive.Header.FormatVersion, Is.EqualTo(1));
			}
		}

		[Test]
		public void Sector_size_shift_is_read_as_expected() {
			using (var archive = OpenTestArchive()) {
				Assert.That(archive.Header.SectorSizeShift, Is.EqualTo(3));
			}
		}

		[Test]
		public void Hash_table_offset_is_read_as_expected() {
			using (var archive = OpenTestArchive()) {
				Assert.That(archive.Header.HashTableOffset, Is.EqualTo(0x0001438c));
			}
		}

		[Test]
		public void Block_table_offset_is_read_as_expected() {
			using (var archive = OpenTestArchive()) {
				Assert.That(archive.Header.BlockTableOffset, Is.EqualTo(0x0001448c));
			}
		}

		[Test]
		public void Hash_table_entry_count_is_read_as_expected() {
			using (var archive = OpenTestArchive()) {
				Assert.That(archive.Header.HashTableEntryCount, Is.EqualTo(16));
			}
		}

		[Test]
		public void Block_table_entry_count_is_read_as_expected() {
			using (var archive = OpenTestArchive()) {
				Assert.That(archive.Header.BlockTableEntryCount, Is.EqualTo(10));
			}
		}
	}
}