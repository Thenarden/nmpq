using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Nmpq.Parsing {
	public static class Deserialization {
		public static JObject ParseSerializedData(BinaryReader reader) {
			if (reader == null) throw new ArgumentNullException("reader");

			var type = (SerializedDataType)reader.ReadByte();

			if (type == SerializedDataType.String) {
				var length = ParseVariableLengthInteger(reader);
				var bytes = reader.ReadBytes(length);
				var str = Encoding.ASCII.GetString(bytes);
				return new JObject(str);
			}

			if (type == SerializedDataType.SingleByteInteger)
				return new JObject(reader.ReadByte());

			if (type == SerializedDataType.FourByteInteger)
				return new JObject(reader.ReadInt32());

			if (type == SerializedDataType.Array) {
				reader.SkipBytes(2);	// arrays are always followed by the bytes 0x01 0x00

				var length = ParseVariableLengthInteger(reader);
				var array = new object[length];

				for (var i = 0; i < length; i++)
					array[i] = ParseSerializedData(reader);

				return new JObject(array);
			}

			if (type == SerializedDataType.Map) {
				var length = ParseVariableLengthInteger(reader);
				var dict = new JObject();

				for (var i = 0; i < length; i++) {
					var key = ParseVariableLengthInteger(reader);
					var value = ParseSerializedData(reader);

					dict[key] = value;
				}

				return dict;
			}

			throw new NotSupportedException(
				string.Format("Serialized data type {0} is not supported.", (byte) type));
		}

		public static int ParseVariableLengthInteger(BinaryReader reader) {
			if (reader == null) throw new ArgumentNullException("reader");
			int value = 0, shift = 0;

			while (true) {
				var @byte = reader.ReadByte();
				value += (@byte & 0x7f) << (shift * 7);

				if ((@byte & 0x80) == 0) {
					break;
				}

				shift++;
			}

			return (value >> 1) * ((value & 0x1) == 0 ? 1 : -1);
		}
	}
}