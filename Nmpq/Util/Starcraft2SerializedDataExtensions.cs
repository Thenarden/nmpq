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

            if (type == Starcraft2SerializedDataType.SingleByteInteger)
                return (int) reader.ReadByte();

            if (type == Starcraft2SerializedDataType.FourByteInteger)
                return reader.ReadInt32();

            if (type == Starcraft2SerializedDataType.VariableLengthInteger)
                return DeserializeVariableLengthInteger(reader);

            if (type == Starcraft2SerializedDataType.BinaryString)
            {
                var length = DeserializeVariableLengthInteger(reader);
                var bytes = reader.ReadBytes((int) length);

                if (convertStringsToUtf8)
                    return Encoding.UTF8.GetString(bytes);

                return bytes;
            }

            if (type == Starcraft2SerializedDataType.Array)
            {
                var flag = reader.ReadByte();

                // first observed in SC2 Patch 1.5:
                //	if the first byte following the array is 0, then the array is empty
                if (flag == 0)
                    return new object[0];

                // this byte has always been observed to be 0, not sure what it means
                reader.ReadByte();

                var length = DeserializeVariableLengthInteger(reader);
                var array = new object[length];

                for (var i = 0; i < length; i++)
                    array[i] = Deserialize(reader, convertStringsToUtf8);

                return array;
            }

            if (type == Starcraft2SerializedDataType.Map)
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

            throw new NotSupportedException(
                string.Format("Serialized data with type flag '{0}' is not supported.", (byte) type));
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