using System.Collections.Generic;
using Nmpq.Parsing;

namespace Nmpq {
	public class HashTable {
		public HashTable(ICollection<HashTableEntry> entries) {
			Entries = entries;
		}

		public ICollection<HashTableEntry> Entries { get; private set; }

		public HashTableEntry? FindEntry(ulong hashA, ulong hashB) {
			foreach(var entry in Entries) {
				if (entry.IsEmpty)
					return null;

				if (hashA == (ulong)entry.FilePathHashA && hashB == (ulong)entry.FilePathHashB)
					return entry;
			}

			return null;
		}
	}
}