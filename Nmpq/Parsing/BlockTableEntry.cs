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
	}
}