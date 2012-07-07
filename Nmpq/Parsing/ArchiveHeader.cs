using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Nmpq.Parsing {
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ArchiveHeader {
		public bool IsMagicValid {
			get {
				var bytes = BitConverter.GetBytes(Magic);
				var mpq = Encoding.ASCII.GetString(bytes, 0, 3);
				return mpq == "MPQ" && bytes[3] == 0x1a;
			}
		}

		public bool IsBurningCrusadeFormat {
			get { return FormatVersion == 1; }
		}

		[FieldOffset(0x00)]
		public int Magic;

		[FieldOffset(0x04)]
		public int HeaderSize;

		[FieldOffset(0x08)]
		public int ArchiveSize;

		[FieldOffset(0x0c)]
		public short FormatVersion;

		[FieldOffset(0x0e)]
		public byte SectorSizeShift;

		[FieldOffset(0x10)]
		public int HashTableOffset;

		[FieldOffset(0x14)]
		public int BlockTableOffset;

		[FieldOffset(0x18)]
		public int HashTableEntryCount;

		[FieldOffset(0x1c)]
		public int BlockTableEntryCount;

		[FieldOffset(0x20)]
		public long ExtendedBlockTableOffset;

		[FieldOffset(0x28)]
		public short HashTableOffsetHigh;

		[FieldOffset(0x2a)]
		public short BlockTableOffsetHigh;
	}
}