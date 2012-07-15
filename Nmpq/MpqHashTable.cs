using System.Collections.Generic;
using System.Linq;
using Nmpq.Parsing;

namespace Nmpq {
	public class MpqHashTable {
		public MpqHashTable(IEnumerable<HashTableEntry> entries) {
			Entries = entries.ToList().AsReadOnly();
		}

		public IList<HashTableEntry> Entries { get; private set; }

		public HashTableEntry? FindEntry(ulong hashA, ulong hashB) {
			foreach(var entry in Entries) {
				if (hashA == (ulong)entry.FilePathHashA && hashB == (ulong)entry.FilePathHashB)
					return entry;
			}

			return null;
		}
	}
}