using NUnit.Framework;

namespace Nmpq.Tests
{
    [TestFixture]
    public class UserDataTests
    {
        [Test]
        public void Can_get_reserved_user_data_size()
        {
            using (var archive = ObjectMother.OpenTestArchive1())
            {
                Assert.That(archive.UserDataHeader.UserDataReservedSize, Is.EqualTo(512));
            }
        }

        [Test]
        public void Can_get_user_data_header_size()
        {
            using (var archive = ObjectMother.OpenTestArchive1())
            {
                Assert.That(archive.UserDataHeader.UserDataSize, Is.EqualTo(60));
            }
        }

        [Test]
        public void Can_read_user_data()
        {
            var expected = TestUtil.LoadBytesFromTextFile("TestArchives/Archive1-UserDataHeader.txt");

            using (var archive = ObjectMother.OpenTestArchive1())
            {
                Assert.That(archive.UserDataHeader.UserData, Is.EqualTo(expected));
            }
        }
    }
}