using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Nmpq.Parsing {
	public static class Deserialization {
		public static object ParseSerializedData(BinaryReader reader) {
			if (reader == null) throw new ArgumentNullException("reader");

			var type = (SerializedDataType)reader.ReadByte();

			if (type == SerializedDataType.SingleByteInteger)
				return (int)reader.ReadByte();

			if (type == SerializedDataType.FourByteInteger)
				return reader.ReadInt32();

			if (type == SerializedDataType.VariableLengthInteger)
				return ParseVariableLengthInteger(reader);

			if (type == SerializedDataType.String) {
				var length = ParseVariableLengthInteger(reader);
				var bytes = reader.ReadBytes((int)length);
				var str = Encoding.UTF8.GetString(bytes);
				return str;
			}

			if (type == SerializedDataType.Array) {
				reader.SkipBytes(2);	// arrays are always followed by the bytes 0x01 0x00

				var length = ParseVariableLengthInteger(reader);
				var array = new object[length];

				for (var i = 0; i < length; i++)
					array[i] = ParseSerializedData(reader);

				return array;
			}

			if (type == SerializedDataType.Map) {
				var length = ParseVariableLengthInteger(reader);
				var dict = new Dictionary<long, object>();

				for (var i = 0; i < length; i++) {
					var key = ParseVariableLengthInteger(reader);
					var value = ParseSerializedData(reader);

					//var keyString = key.ToString(NumberFormatInfo.InvariantInfo);
					dict[key] = value;
				}

				return dict;
			}

			throw new NotSupportedException(
				string.Format("Serialized data with type flag '{0}' is not supported.", (byte) type));
		}

		public static long ParseVariableLengthInteger(BinaryReader reader) {
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