using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Nmpq {
	public static class MpqSerializedData {
		public static object Deserialize(byte[] data, bool convertStringsToUtf8) {
			if (data == null) throw new ArgumentNullException("data");

			using (var memory = new MemoryStream(data))
			using (var reader = new BinaryReader(memory))
				return Deserialize(reader, convertStringsToUtf8);
		}

		public static object Deserialize(BinaryReader reader, bool convertStringsToUtf8) {
			if (reader == null) throw new ArgumentNullException("reader");

			var type = (MpqSerializedDataType)reader.ReadByte();

			if (type == MpqSerializedDataType.SingleByteInteger)
				return (int)reader.ReadByte();

			if (type == MpqSerializedDataType.FourByteInteger)
				return reader.ReadInt32();

			if (type == MpqSerializedDataType.VariableLengthInteger)
				return DeserializeVariableLengthInteger(reader);

			if (type == MpqSerializedDataType.BinaryString) {
				var length = DeserializeVariableLengthInteger(reader);
				var bytes = reader.ReadBytes((int)length);

				if (convertStringsToUtf8)
					return Encoding.UTF8.GetString(bytes);

				return bytes;
			}

			if (type == MpqSerializedDataType.Array) {
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

			if (type == MpqSerializedDataType.Map) {
				var length = DeserializeVariableLengthInteger(reader);
				var dict = new Dictionary<long, object>();

				for (var i = 0; i < length; i++) {
					var key = DeserializeVariableLengthInteger(reader);
					var value = Deserialize(reader, convertStringsToUtf8);
					dict[key] = value;
				}

				return dict;
			}

			throw new NotSupportedException(
				string.Format("Serialized data with type flag '{0}' is not supported.", (byte) type));
		}

		public static long DeserializeVariableLengthInteger(BinaryReader reader) {
			if (reader == null) throw new ArgumentNullException("reader");
			long value = 0;
			var shift = 0;

			while (true) {
				long @byte = reader.ReadByte();
				value |= (@byte & 0x7f) << (shift * 7);

				if ((@byte & 0x80) == 0) {
					break;
				}

				shift++;
			}

			return (value >> 1) * ((value & 0x1) == 0 ? 1 : -1);
		}
	}
}