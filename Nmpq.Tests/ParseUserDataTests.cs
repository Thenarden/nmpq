using NUnit.Framework;

namespace Nmpq.Tests
{
    [TestFixture]
    public class ParseUserDataTests
    {
        private const string Testfile = "TestArchives/Archive1.SC2Replay";

        [Test]
        public void Can_get_reserved_user_data_size()
        {
            var archive = ObjectMother.ReadTestFile(Testfile, "2gMBq3cvcyaO2PLK7QWjuiigSQE=");
            var userData = MpqArchive.ParseUserDataHeader(archive);

            Assert.That(userData.UserDataReservedSize, Is.EqualTo(512));
        }

        [Test]
        public void Can_get_user_data_header_size()
        {
            var archive = ObjectMother.ReadTestFile(Testfile, "2gMBq3cvcyaO2PLK7QWjuiigSQE=");
            var userData = MpqArchive.ParseUserDataHeader(archive);
            Assert.That(userData.UserDataSize, Is.EqualTo(60));
        }

        [Test]
        public void Can_read_user_data()
        {
            var expected = TestUtil.LoadBytesFromTextFile("TestArchives/Archive1-UserDataHeader.txt");
            var archive = ObjectMother.ReadTestFile(Testfile, "2gMBq3cvcyaO2PLK7QWjuiigSQE=");
            var userData = MpqArchive.ParseUserDataHeader(archive);

            Assert.That(userData.UserData, Is.EqualTo(expected));
        }
    }
}