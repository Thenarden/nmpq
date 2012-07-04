using System.Collections.Generic;
using Nmpq.Parsing;

namespace Nmpq {
	public class BlockTable {
		public BlockTable(ICollection<BlockTableEntry> entries) {
			Entries = entries;
		}

		public ICollection<BlockTableEntry> Entries { get; private set; }
	}
}