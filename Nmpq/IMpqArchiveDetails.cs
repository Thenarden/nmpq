using Nmpq.Parsing;

namespace Nmpq {
	public interface IMpqArchiveDetails {
		ArchiveHeader ArchiveHeader { get; }
		MpqHashTable HashTable { get; }
		BlockTableEntry[] BlockTable { get;  }

		int SectorSize { get; }
		int ArchiveOffset { get; }

		int UserDataMaxSize { get; }
		int UserDataActualSize { get; }
	}
}