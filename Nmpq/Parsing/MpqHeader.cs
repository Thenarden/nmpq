using System.Runtime.InteropServices;

namespace Nmpq.Parsing {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MpqHeader {
		public bool IsMagicValid {
			get {
				return Magic[0] == (byte) 'M'
					&& Magic[1] == (byte) 'P'
					&& Magic[2] == (byte) 'Q'
					&& Magic[3] == 0x1a;
			}
		}

		public bool IsBurningCrusadeFormat {
			get { return FormatVersion == 1; }
		}

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] Magic;

		public int HeaderSize;
		public int ArchiveSize;
		public short FormatVersion;
		public byte SectorSizeShift;
		public int HashTableOffset;
		public int BlockTableOffset;
		public int HashTableEntires;
		public int BlockTableEntries;

		public long ExtendedBlockTableOffset;
		public short HashTableOffsetHigh;
		public short BlockTableOffsetHigh;
	}
}