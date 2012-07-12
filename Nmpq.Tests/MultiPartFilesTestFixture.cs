using NUnit.Framework;

namespace Nmpq.Tests {
	[TestFixture]
	public class MultiPartFilesTestFixture {
		[Test]
		public void Multi_part_file_data_is_read_successfully() {
			var expected = ObjectMother.GetTestArchive2Minimap();

			using (var archive = ObjectMother.OpenTestArchive2()) {
				var file = archive.ExtractFileBytes("Minimap.tga");

				Assert.That(file, Is.EqualTo(expected));
			}
		}
	}
}