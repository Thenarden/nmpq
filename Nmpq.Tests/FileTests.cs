using System.Text;
using NUnit.Framework;

namespace Nmpq.Tests
{
    [TestFixture]
    public class FileTests
    {
        [Test]
        public void Open_returns_data_for_specified_file_if_it_exists_in_archive()
        {
            using (var archive = ObjectMother.OpenTestArchive1())
            {
                var listfile = archive.ReadFile("(listfile)");

                Assert.That(listfile, Is.Not.Null);
            }
        }

        [Test]
        public void Listfile_contains_expected_data()
        {
            var expectedListfileContents = @"replay.attributes.events
replay.details
replay.game.events
replay.initData
replay.load.info
replay.message.events
replay.smartcam.events
replay.sync.events
";

            using (var archive = ObjectMother.OpenTestArchive1())
            {
                var listfile = archive.ReadFile("(listfile)");
                Assume.That(listfile, Is.Not.Null.And.Not.Empty);

                var listfileContents = Encoding.UTF8.GetString(listfile);
                Assert.That(listfileContents, Is.EqualTo(expectedListfileContents));
            }
        }

        [Test]
        public void Can_read_files_that_are_marked_as_compressed_but_arent_compressed()
        {
            var expectedBytes = TestUtil.ParseBytes(
                @"00 22 80 00 00 0B 01 00 22 80 00 00 0F 00 00 22 80 00 00 13 00 00 
					  22 80 00 00 17 01 00 22 80 00 00 1B 01 00 22 80 00 00 1E 00 00 22 
					  80 00 00 32 00 01 B2 02 00 04 67 6C 68 66 01 CE 01 00 02 75 32");

            using (var archive = ObjectMother.OpenTestArchive1())
            {
                var file = archive.ReadFile("replay.message.events");

                Assert.That(file, Is.EqualTo(expectedBytes));
            }
        }

        [Test]
        public void Files_using_deflate_compression_are_opened_successfully()
        {
            var expectedListfileContents = @"Base.SC2Data\GameData\TerrainData.xml
Base.SC2Data\GameData\WaterData.xml
ComponentList.SC2Components
deDE.SC2Data\LocalizedData\GameStrings.txt
DocumentHeader
DocumentInfo
DocumentInfo.version
enUS.SC2Data\LocalizedData\GameStrings.txt
esES.SC2Data\LocalizedData\GameStrings.txt
esMX.SC2Data\LocalizedData\GameStrings.txt
frFR.SC2Data\LocalizedData\GameStrings.txt
GameData.version
GameText.version
itIT.SC2Data\LocalizedData\GameStrings.txt
koKR.SC2Data\LocalizedData\GameStrings.txt
koKR.SC2Data\LocalizedData\TriggerStrings.txt
MapInfo
MapInfo.version
MapScript.galaxy
Minimap.tga
Objects
Objects.version
PaintedPathingLayer
plPL.SC2Data\LocalizedData\GameStrings.txt
Preload.xml
ptBR.SC2Data\LocalizedData\GameStrings.txt
ruRU.SC2Data\LocalizedData\GameStrings.txt
t3CellFlags
t3FluffDoodad
t3HardTile
t3HeightMap
t3SyncCliffLevel
t3SyncHeightMap
t3Terrain.version
t3Terrain.xml
t3TextureMasks
t3VertCol
t3Water
Triggers
Triggers.version
zhCN.SC2Data\LocalizedData\GameStrings.txt
zhTW.SC2Data\LocalizedData\GameStrings.txt
";

            using (var archive = ObjectMother.OpenTestArchive2())
            {
                var listfile = archive.ReadFile("(listfile)");
                Assume.That(listfile, Is.Not.Null.And.Not.Empty);

                var listfileContents = Encoding.UTF8.GetString(listfile);
                Assert.That(listfileContents, Is.EqualTo(expectedListfileContents));
            }
        }
    }
}