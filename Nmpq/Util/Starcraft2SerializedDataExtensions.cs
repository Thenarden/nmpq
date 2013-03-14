using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Nmpq.Util
{
    /// <summary>
    /// Parses Starcraft 2's JSON-like data format, which is used within replay files.
    /// 
    /// This will probably be moved out to a separate library at some point, since it's
    /// not actually part of the core MPQ spec.
    /// 
    /// More information about the data format can be found at http://www.teamliquid.net/forum/viewmessage.php?topic_id=117260&currentpage=3#45
    /// </summary>
    public static class Starcraft2SerializedDataExtensions
    {
        public static object ReadSerializedData(this IMpqArchive archive, string path, bool convertStringsToUtf8)
        {
            if (archive == null) throw new ArgumentNullException("archive");

            var file = archive.ReadFile(path);

            if (file == null)
                return null;

            using (var memory = new MemoryStream(file))
            using (var reader = new BinaryReader(memory))
            {
                return Deserialize(reader, convertStringsToUtf8);
            }
        }

        public static object Deserialize(byte[] data, bool convertStringsToUtf8)
        {
            if (data == null) throw new ArgumentNullException("data");

            using (var memory = new MemoryStream(data))
            using (var reader = new BinaryReader(memory))
                return Deserialize(reader, convertStringsToUtf8);
        }

        public static object Deserialize(BinaryReader reader, bool convertStringsToUtf8)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            var type = (Starcraft2SerializedDataType) reader.ReadByte();
            return Deserialize(type, reader, convertStringsToUtf8);
        }

        private static object Deserialize(Starcraft2SerializedDataType type, BinaryReader reader, bool convertStringsToUtf8)
        {
            switch (type)
            {
                case Starcraft2SerializedDataType.SingleByteInteger:
                    return (int)reader.ReadByte();
                case Starcraft2SerializedDataType.FourByteInteger:
                    return reader.ReadInt32();
                case Starcraft2SerializedDataType.VariableLengthInteger:
                    return DeserializeVariableLengthInteger(reader);
                case Starcraft2SerializedDataType.BinaryString:
                    {
                        var length = DeserializeVariableLengthInteger(reader);
                        var bytes = reader.ReadBytes((int)length);

                        if (convertStringsToUtf8)
                            return Encoding.UTF8.GetString(bytes);

                        return bytes;
                    }
                case Starcraft2SerializedDataType.Array:
                    {
                        var flag = reader.ReadByte();

                        // first observed in SC2 Patch 1.5:
                        //	if the first byte following the array is 0, then the array is empty
                        if (flag == 0)
                            return new object[0];

                        // first observed in HOTS Patch 2.0:
                        //  the byte immediately following the flag appears to be
                        //  an data type indicator - If it's not null, then we
                        //  assume the array has a length of 1 and parse the single
                        //  value according to the data type in this byte.
                        var singleElementType = reader.ReadByte();

                        if (singleElementType != 0)
                        {
                            return new [] { Deserialize((Starcraft2SerializedDataType)singleElementType, reader, convertStringsToUtf8) };
                        }
                        else
                        {
                            var length = DeserializeVariableLengthInteger(reader);
                            var array = new object[length];

                            for (var i = 0; i < length; i++)
                                array[i] = Deserialize(reader, convertStringsToUtf8);

                            return array;                            
                        }

                    }
                case Starcraft2SerializedDataType.Map:
                    {
                        var length = DeserializeVariableLengthInteger(reader);
                        var dict = new Dictionary<long, object>();

                        for (var i = 0; i < length; i++)
                        {
                            var key = DeserializeVariableLengthInteger(reader);
                            var value = Deserialize(reader, convertStringsToUtf8);
                            dict[key] = value;
                        }

                        return dict;
                    }
                default:
                    throw new MpqParsingException(
                        string.Format("Serialized data with type flag '{0}' is not supported.", (byte)type));
            }
        }

        public static long DeserializeVariableLengthInteger(BinaryReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            long value = 0;
            var shift = 0;

            while (true)
            {
                long @byte = reader.ReadByte();
                value |= (@byte & 0x7f) << (shift*7);

                if ((@byte & 0x80) == 0)
                {
                    break;
                }

                shift++;
            }

            return (value >> 1)*((value & 0x1) == 0 ? 1 : -1);
        }
    }
}