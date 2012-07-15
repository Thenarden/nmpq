using NUnit.Framework;

namespace Nmpq.Tests {
	[TestFixture]
	public class UserDataTests {
		[Test]
		public void Can_get_max_user_data_size() {
			using (var archive = ObjectMother.OpenTestArchive1()) {
				Assert.That(archive.UserDataMaxSize, Is.EqualTo(512));
			}
		}
		[Test]
		public void Can_get_user_data_header_size() {
			using (var archive = ObjectMother.OpenTestArchive1()) {
				Assert.That(archive.UserDataHeaderSize, Is.EqualTo(60));
			}
		}

		[Test]
		public void Can_read_user_data() {
			var expected = TestUtil.LoadBytesFromTextFile("TestArchives/Archive1-UserDataHeader.txt");

			using(var archive = ObjectMother.OpenTestArchive1()) {
				Assert.That(archive.UserDataHeader, Is.EqualTo(expected));
			}
		}
	}
}