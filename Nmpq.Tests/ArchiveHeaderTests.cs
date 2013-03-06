using NUnit.Framework;

namespace Nmpq.Tests
{
    [TestFixture]
    public class ArchiveHeaderTests
    {
        [TestCaseSource(typeof (ObjectMother), "AllArchives")]
        public void Magic_is_read_as_expected(string filename)
        {
            using (var archive = ObjectMother.OpenTestArchive(filename))
            {
                var magic = archive.Details.ArchiveHeader.Magic;

                Assert.That(magic, Is.EqualTo(0x1a51504d));
            }
        }

        [TestCaseSource(typeof (ObjectMother), "AllArchives")]
        public void Header_size_is_read_as_expected(string filename)
        {
            using (var archive = ObjectMother.OpenTestArchive(filename))
            {
                Assert.That(archive.Details.ArchiveHeader.HeaderSize, Is.EqualTo(0x2c));
            }
        }

        [TestCaseSource(typeof (ObjectMother), "AllArchives")]
        public void Archive_size_is_read_as_expected(string filename)
        {
            using (var archive = ObjectMother.OpenTestArchive(filename))
            {
                Assert.That(archive.Details.ArchiveHeader.ArchiveSize, Is.EqualTo(83244));
            }
        }

        [TestCaseSource(typeof (ObjectMother), "AllArchives")]
        public void Format_is_read_as_expected(string filename)
        {
            using (var archive = ObjectMother.OpenTestArchive(filename))
            {
                Assert.That(archive.Details.ArchiveHeader.FormatVersion, Is.EqualTo(1));
            }
        }

        [TestCaseSource(typeof (ObjectMother), "AllArchives")]
        public void Sector_size_shift_is_read_as_expected(string filename)
        {
            using (var archive = ObjectMother.OpenTestArchive(filename))
            {
                Assert.That(archive.Details.ArchiveHeader.SectorSizeShift, Is.EqualTo(3));
            }
        }

        [TestCaseSource(typeof (ObjectMother), "AllArchives")]
        public void Hash_table_offset_is_read_as_expected(string filename)
        {
            using (var archive = ObjectMother.OpenTestArchive(filename))
            {
                Assert.That(archive.Details.ArchiveHeader.HashTableOffset, Is.EqualTo(0x0001438c));
            }
        }

        [TestCaseSource(typeof (ObjectMother), "AllArchives")]
        public void Block_table_offset_is_read_as_expected(string filename)
        {
            using (var archive = ObjectMother.OpenTestArchive(filename))
            {
                Assert.That(archive.Details.ArchiveHeader.BlockTableOffset, Is.EqualTo(0x0001448c));
            }
        }

        [TestCaseSource(typeof (ObjectMother), "AllArchives")]
        public void Hash_table_entry_count_is_read_as_expected(string filename)
        {
            using (var archive = ObjectMother.OpenTestArchive(filename))
            {
                Assert.That(archive.Details.ArchiveHeader.HashTableEntryCount, Is.EqualTo(16));
            }
        }

        [TestCaseSource(typeof (ObjectMother), "AllArchives")]
        public void Block_table_entry_count_is_read_as_expected(string filename)
        {
            using (var archive = ObjectMother.OpenTestArchive(filename))
            {
                Assert.That(archive.Details.ArchiveHeader.BlockTableEntryCount, Is.EqualTo(10));
            }
        }

        [TestCaseSource(typeof (ObjectMother), "AllArchives")]
        public void SectorSize_is_calculated_correctly(string filename)
        {
            using (var archive = (MpqArchive) ObjectMother.OpenTestArchive(filename))
            {
                Assert.That(archive.SectorSize, Is.EqualTo(4096));
            }
        }
    }
}