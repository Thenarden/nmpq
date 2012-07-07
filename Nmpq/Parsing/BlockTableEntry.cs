using System.Runtime.InteropServices;

namespace Nmpq.Parsing {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BlockTableEntry {
		public int BlockOffset;
		public int BlockSize;
		public int FileSize;
		public BlockFlags Flags;

		public bool HasKeyAdjustedByBlockOffset {
			get { return (Flags & BlockFlags.KeyIsAdjustedByBlockOffset) != 0;  }
		}

		public bool IsFile {
			get { return (Flags & BlockFlags.IsFile) != 0; }
		}

		public bool IsFileSingleUnit {
			get { return (Flags & BlockFlags.FileIsSingleUnit) != 0; }
		}

		public bool IsCompressed {
			get { return (Flags & BlockFlags.FileIsCompressed) != 0; }
		}

		public bool IsEncrypted {
			get { return (Flags & BlockFlags.FileIsEncrypted) != 0; }
		}

		public bool IsImploded {
			get { return (Flags & BlockFlags.FileIsImploded) != 0; }
		}
	}
}