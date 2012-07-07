using NUnit.Framework;

namespace Nmpq.Tests {
	[TestFixture]
	public class MultiPartFilesTestFixture {
		[Test]
		public void Multi_part_file_data_is_read_successfully() {
			using (var archive = TestArchiveFactory.OpenTestArchive2()) {
				var file = archive.ReadFileBytes("Minimap.tga");
			}
		}
	}
}