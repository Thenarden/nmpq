using NUnit.Framework;

namespace Nmpq.Tests
{
    [TestFixture]
    public class MultiPartFilesTests
    {
        [Test]
        public void Can_read_multipart_file()
        {
            var expected = ObjectMother.GetTestArchive2Minimap();

            using (var archive = ObjectMother.OpenTestArchive2())
            {
                var file = archive.ReadFile("Minimap.tga");

                Assert.That(file, Is.EqualTo(expected));
            }
        }
    }
}