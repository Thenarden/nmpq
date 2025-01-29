using Nmpq.Util;
using NUnit.Framework;

namespace Nmpq.Tests
{
    [TestFixture]
    public class DeserializationTests
    {
        [Test]
        public void Can_parse_serialized_data_from_heart_of_the_swarm_replay()
        {
            using (var archive = ObjectMother.OpenHotsReplay())
            {
                dynamic replayDetails = archive.ReadSerializedData("replay.details", true);

                Assert.That(replayDetails, Is.Not.Null);
                Assert.That(replayDetails[0].Length, Is.EqualTo(2));

                Assert.That(replayDetails[0][0][0], Is.EqualTo("[LGS]<sp/>LuIsCoBrA"));
                Assert.That(replayDetails[0][0][1][2], Is.EqualTo(1));
                Assert.That(replayDetails[0][0][1][4], Is.EqualTo(1871860));
                Assert.That(replayDetails[0][0][2], Is.EqualTo("Zerg"));

                Assert.That(replayDetails[0][1][0], Is.EqualTo("[WOOT]<sp/>DeadBabySeal"));
                Assert.That(replayDetails[0][1][1][2], Is.EqualTo(1));
                Assert.That(replayDetails[0][1][1][4], Is.EqualTo(2991693));
                Assert.That(replayDetails[0][1][2], Is.EqualTo("Zerg"));

                Assert.That(replayDetails[1], Is.EqualTo("Whirlwind LE"));
            }
        }

        [Test]
        public void Can_parse_serialized_data_from_sc2_replay()
        {
            using (var archive = ObjectMother.OpenTestArchive1())
            {
                dynamic replayDetails = archive.ReadSerializedData("replay.details", true);

                Assert.That(replayDetails, Is.Not.Null);
                Assert.That(replayDetails[0].Length, Is.EqualTo(2));

                Assert.That(replayDetails[0][0][0], Is.EqualTo("DeadBabySeal"));
                Assert.That(replayDetails[0][0][1][2], Is.EqualTo(1));
                Assert.That(replayDetails[0][0][1][4], Is.EqualTo(2991693));
                Assert.That(replayDetails[0][0][2], Is.EqualTo("Zerg"));

                Assert.That(replayDetails[0][1][0], Is.EqualTo("OverDrive"));
                Assert.That(replayDetails[0][1][1][2], Is.EqualTo(1));
                Assert.That(replayDetails[0][1][1][4], Is.EqualTo(947366));
                Assert.That(replayDetails[0][1][2], Is.EqualTo("Terran"));

                Assert.That(replayDetails[1], Is.EqualTo("Ohana LE"));
            }
        }

        [Test]
        public void ReadSerializedData_returns_null_for_nonexistant_file()
        {
            using (var archive = ObjectMother.OpenTestArchive1())
            {
                var file = archive.ReadSerializedData("a file that doesn't exist", true);
                Assert.That(file, Is.Null);
            }
        }

        // format information and test values taken from http://www.teamliquid.net/forum/viewmessage.php?topic_id=117260&currentpage=3#45
        [TestCase(new byte[] {0x00}, 0)]
        [TestCase(new byte[] {0x01}, 0)]
        [TestCase(new byte[] {0x02}, 1)]
        [TestCase(new byte[] {0x03}, -1)]
        [TestCase(new byte[] {0x7e}, 63)]
        [TestCase(new byte[] {0x7f}, -63)]
        [TestCase(new byte[] {0x80, 0x00}, 0)]
        [TestCase(new byte[] {0x80, 0x01}, 64)]
        [TestCase(new byte[] {0x81, 0x01}, -64)]
        [TestCase(new byte[] {0x82, 0x01}, 65)]
        [TestCase(new byte[] {0x83, 0x01}, -65)]
        [TestCase(new byte[] {0x80, 0x02}, 128)]
        [TestCase(new byte[] {0x81, 0x02}, -128)]
        [TestCase(new byte[] {0x80, 0x80, 0x80, 0x10}, 16777216)]
        [TestCase(new byte[] {0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x10}, 4503599627370496)]
        public void Variable_length_integers_are_deserialized_correctly(byte[] bytes, long expectedValue)
        {
            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream);
            
            var value = Starcraft2SerializedData.DeserializeVariableLengthInteger(reader);
            Assert.That(value, Is.EqualTo(expectedValue));
        }

        [TestCase(new byte[] {0x80}, 0, typeof(EndOfStreamException))]
        public void Variable_length_integers_expected_exception(byte[] bytes, long expectedValue, Type expectedException)
        {
            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream);
            Assert.Throws(expectedException, () => Starcraft2SerializedData.DeserializeVariableLengthInteger(reader));
        }
        
        [Test]
        public void Unknown_data_type_causes_MpqParsingException()
        {
            var bytes = new byte[] {0xFF};
            using (var stream = new MemoryStream(bytes))
            using (var reader = new BinaryReader(stream))
            {
                Assert.Throws<MpqParsingException>(
                    () => Starcraft2SerializedData.Deserialize(reader, false));
            }
        }
    }
}