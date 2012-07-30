using NUnit.Framework;

namespace Nmpq.Tests {
	[TestFixture]
	public class UserDataTests {
		[Test]
		public void Can_get_reserved_user_data_size() {
			using (var archive = ObjectMother.OpenTestArchive1()) {
				Assert.That(archive.UserDataReservedSize, Is.EqualTo(512));
			}
		}
		[Test]
		public void Can_get_user_data_header_size() {
			using (var archive = ObjectMother.OpenTestArchive1()) {
				Assert.That(archive.UserDataSize, Is.EqualTo(60));
			}
		}

		[Test]
		public void Can_read_user_data() {
			var expected = TestUtil.LoadBytesFromTextFile("TestArchives/Archive1-UserDataHeader.txt");

			using(var archive = ObjectMother.OpenTestArchive1()) {
				Assert.That(archive.UserData, Is.EqualTo(expected));
			}
		}
	}
}