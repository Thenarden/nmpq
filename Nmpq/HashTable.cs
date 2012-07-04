using System.Collections.Generic;
using Nmpq.Parsing;

namespace Nmpq {
	public class HashTable {
		public HashTable(ICollection<HashTableEntry> entries) {
			Entries = entries;
		}

		public ICollection<HashTableEntry> Entries { get; private set; }
	}
}