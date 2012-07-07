using System.IO;

namespace Nmpq.Parsing {
	public static class Deserialization {
		public static int ParseVariableLengthInteger(BinaryReader reader) {
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