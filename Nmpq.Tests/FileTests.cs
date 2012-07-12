using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Nmpq.Tests {
	[TestFixture]
	public class FileTests {
		[Test]
		public void Open_returns_data_for_specified_file_if_it_exists_in_archive() {
			using(var archive = TestArchiveFactory.OpenTestArchive1()) {
				var listfile = archive.ExtractFileBytes("(listfile)");

				Assert.That(listfile, Is.Not.Null);
			}
		}

		[Test]
		public void Listfile_contains_expected_data() {
			var expectedListfileContents = @"replay.attributes.events
replay.details
replay.game.events
replay.initData
replay.load.info
replay.message.events
replay.smartcam.events
replay.sync.events
";

			using(var archive = TestArchiveFactory.OpenTestArchive1()) {
				var listfile = archive.ExtractFileBytes("(listfile)");
				Assume.That(listfile, Is.Not.Null.And.Not.Empty);

				var listfileContents = Encoding.UTF8.GetString(listfile);
				Assert.That(listfileContents, Is.EqualTo(expectedListfileContents));
			}
		}

		[Test]
		public void Files_using_deflate_compression_are_opened_successfully() {
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

			using(var archive = TestArchiveFactory.OpenTestArchive2()) {
				var listfile = archive.ExtractFileBytes("(listfile)");
				Assume.That(listfile, Is.Not.Null.And.Not.Empty);

				var listfileContents = Encoding.UTF8.GetString(listfile);
				Assert.That(listfileContents, Is.EqualTo(expectedListfileContents));
			}
		}
	}
}