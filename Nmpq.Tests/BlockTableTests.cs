using NUnit.Framework;
using Nmpq.Parsing;

namespace Nmpq.Tests
{
    [TestFixture]
    public class BlockTableTests
    {
        [Test]
        public void Block_table_is_read_correctly()
        {
            using (var archive = (MpqArchive) ObjectMother.OpenTestArchive1())
            {
                var table = archive.BlockTable;

                Assert.That(table.Count, Is.EqualTo(10));

                var expectedFlags = BlockFlags.IsFile | BlockFlags.FileIsCompressed | BlockFlags.FileIsSingleUnit;
                Assert.That(table[0].BlockOffset, Is.EqualTo(0x0000002C));
                Assert.That(table[0].BlockSize, Is.EqualTo(0x000001C0));
                Assert.That(table[0].FileSize, Is.EqualTo(0x000001C0));
                Assert.That(table[0].Flags, Is.EqualTo(expectedFlags));

                Assert.That(table[1].BlockOffset, Is.EqualTo(0x000001EC));
                Assert.That(table[1].BlockSize, Is.EqualTo(0x00000289));
                Assert.That(table[1].FileSize, Is.EqualTo(0x000004D9));
                Assert.That(table[1].Flags, Is.EqualTo(expectedFlags));

                Assert.That(table[2].BlockOffset, Is.EqualTo(0x00000475));
                Assert.That(table[2].BlockSize, Is.EqualTo(0x00011644));
                Assert.That(table[2].FileSize, Is.EqualTo(0x0001FF45));
                Assert.That(table[2].Flags, Is.EqualTo(expectedFlags));

                Assert.That(table[3].BlockOffset, Is.EqualTo(0x00011AB9));
                Assert.That(table[3].BlockSize, Is.EqualTo(0x00000041));
                Assert.That(table[3].FileSize, Is.EqualTo(0x00000041));
                Assert.That(table[3].Flags, Is.EqualTo(expectedFlags));

                Assert.That(table[4].BlockOffset, Is.EqualTo(0x00011AFA));
                Assert.That(table[4].BlockSize, Is.EqualTo(0x00000060));
                Assert.That(table[4].FileSize, Is.EqualTo(0x00000060));
                Assert.That(table[4].Flags, Is.EqualTo(expectedFlags));

                Assert.That(table[5].BlockOffset, Is.EqualTo(0x00011B5A));
                Assert.That(table[5].BlockSize, Is.EqualTo(0x00000550));
                Assert.That(table[5].FileSize, Is.EqualTo(0x000007FD));
                Assert.That(table[5].Flags, Is.EqualTo(expectedFlags));

                Assert.That(table[6].BlockOffset, Is.EqualTo(0x000120AA));
                Assert.That(table[6].BlockSize, Is.EqualTo(0x00002060));
                Assert.That(table[6].FileSize, Is.EqualTo(0x0000A0F2));
                Assert.That(table[6].Flags, Is.EqualTo(expectedFlags));

                Assert.That(table[7].BlockOffset, Is.EqualTo(0x0001410A));
                Assert.That(table[7].BlockSize, Is.EqualTo(0x000000F2));
                Assert.That(table[7].FileSize, Is.EqualTo(0x00000211));
                Assert.That(table[7].Flags, Is.EqualTo(expectedFlags));

                Assert.That(table[8].BlockOffset, Is.EqualTo(0x000141FC));
                Assert.That(table[8].BlockSize, Is.EqualTo(0x00000078));
                Assert.That(table[8].FileSize, Is.EqualTo(0x000000A4));
                Assert.That(table[8].Flags, Is.EqualTo(expectedFlags));

                Assert.That(table[9].BlockOffset, Is.EqualTo(0x00014274));
                Assert.That(table[9].BlockSize, Is.EqualTo(0x00000118));
                Assert.That(table[9].FileSize, Is.EqualTo(0x00000120));
                Assert.That(table[9].Flags, Is.EqualTo(expectedFlags));
            }
        }
    }
}