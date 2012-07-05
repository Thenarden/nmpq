using System.IO;
using NUnit.Framework;

namespace Nmpq.Tests {
	[TestFixture]
	public class HashTableTests {
		[Test]
		public void Hash_table_is_read_correctly() {
			var path = "TestArchives/Archive1.SC2Replay";
			Assume.That(File.Exists(path));

			using (var archive = MpqArchive.Open(path)) {
				var table = archive.HashTable;

				Assert.That(table.Entries[0].FilePathHashA, Is.EqualTo(0xD38437CB));
				Assert.That(table.Entries[0].FilePathHashB, Is.EqualTo(0x07DFEAEC));
				Assert.That(table.Entries[0].FileBlockIndex, Is.EqualTo(0x00000009));
				Assert.That(table.Entries[0].Language, Is.EqualTo(0));
				Assert.That(table.Entries[0].Platform, Is.EqualTo(0));

				Assert.That(table.Entries[1].FilePathHashA, Is.EqualTo(0xAAC2A54B));
				Assert.That(table.Entries[1].FilePathHashB, Is.EqualTo(0xF4762B95));
				Assert.That(table.Entries[1].FileBlockIndex, Is.EqualTo(0x00000002));
				Assert.That(table.Entries[1].Language, Is.EqualTo(0));
				Assert.That(table.Entries[1].Platform, Is.EqualTo(0));

				Assert.That(table.Entries[2].IsEmpty, Is.True);
				Assert.That(table.Entries[3].IsEmpty, Is.True);
				Assert.That(table.Entries[4].IsEmpty, Is.True);

				Assert.That(table.Entries[5].FilePathHashA, Is.EqualTo(0xC9E5B770));
				Assert.That(table.Entries[5].FilePathHashB, Is.EqualTo(0x3B18F6B6));
				Assert.That(table.Entries[5].FileBlockIndex, Is.EqualTo(0x00000005));
				Assert.That(table.Entries[5].Language, Is.EqualTo(0));
				Assert.That(table.Entries[5].Platform, Is.EqualTo(0));

				Assert.That(table.Entries[6].FilePathHashA, Is.EqualTo(0x343C087B));
				Assert.That(table.Entries[6].FilePathHashB, Is.EqualTo(0x278E3682));
				Assert.That(table.Entries[6].FileBlockIndex, Is.EqualTo(0x00000004));
				Assert.That(table.Entries[6].Language, Is.EqualTo(0));
				Assert.That(table.Entries[6].Platform, Is.EqualTo(0));

				Assert.That(table.Entries[7].FilePathHashA, Is.EqualTo(0x3B2B1EA0));
				Assert.That(table.Entries[7].FilePathHashB, Is.EqualTo(0xB72EF057));
				Assert.That(table.Entries[7].FileBlockIndex, Is.EqualTo(0x00000006));
				Assert.That(table.Entries[7].Language, Is.EqualTo(0));
				Assert.That(table.Entries[7].Platform, Is.EqualTo(0));

				Assert.That(table.Entries[8].FilePathHashA, Is.EqualTo(0x5A7E8BDC));
				Assert.That(table.Entries[8].FilePathHashB, Is.EqualTo(0xFF253F5C));
				Assert.That(table.Entries[8].FileBlockIndex, Is.EqualTo(0x00000001));
				Assert.That(table.Entries[8].Language, Is.EqualTo(0));
				Assert.That(table.Entries[8].Platform, Is.EqualTo(0));

				Assert.That(table.Entries[9].FilePathHashA, Is.EqualTo(0xFD657910));
				Assert.That(table.Entries[9].FilePathHashB, Is.EqualTo(0x4E9B98A7));
				Assert.That(table.Entries[9].FileBlockIndex, Is.EqualTo(0x00000008));
				Assert.That(table.Entries[9].Language, Is.EqualTo(0));
				Assert.That(table.Entries[9].Platform, Is.EqualTo(0));

				Assert.That(table.Entries[10].FilePathHashA, Is.EqualTo(0xD383C29C));
				Assert.That(table.Entries[10].FilePathHashB, Is.EqualTo(0xEF402E92));
				Assert.That(table.Entries[10].FileBlockIndex, Is.EqualTo(0x00000000));
				Assert.That(table.Entries[10].Language, Is.EqualTo(0));
				Assert.That(table.Entries[10].Platform, Is.EqualTo(0));

				Assert.That(table.Entries[11].IsEmpty, Is.True);
				Assert.That(table.Entries[12].IsEmpty, Is.True);
				Assert.That(table.Entries[13].IsEmpty, Is.True);

				Assert.That(table.Entries[14].FilePathHashA, Is.EqualTo(0x1DA8B0CF));
				Assert.That(table.Entries[14].FilePathHashB, Is.EqualTo(0xA2CEFF28));
				Assert.That(table.Entries[14].FileBlockIndex, Is.EqualTo(0x00000007));
				Assert.That(table.Entries[14].Language, Is.EqualTo(0));
				Assert.That(table.Entries[14].Platform, Is.EqualTo(0));

				Assert.That(table.Entries[15].FilePathHashA, Is.EqualTo(0x31952289));
				Assert.That(table.Entries[15].FilePathHashB, Is.EqualTo(0x6A5FFAA3));
				Assert.That(table.Entries[15].FileBlockIndex, Is.EqualTo(0x00000003));
				Assert.That(table.Entries[15].Language, Is.EqualTo(0));
				Assert.That(table.Entries[15].Platform, Is.EqualTo(0));

			}
		}
	}
}