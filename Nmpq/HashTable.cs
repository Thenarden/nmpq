using System.Collections.Generic;
using Nmpq.Parsing;

namespace Nmpq {
	public class HashTable {
		public HashTable(ICollection<HashTableEntry> entries) {
			Entries = entries;
		}

		public ICollection<HashTableEntry> Entries { get; private set; }

		public HashTableEntry? FindEntry(ulong hashA, ulong hashB) {
			// these casts scare me. are hashes supposed to be 64 bits? 
			//	the documentation here http://wiki.devklog.net/index.php?title=MPQ_format_specification seems to be inconsistent
			//	they specify that the hashes on the hash table entries are 32-bits, but 
			//		the hash algortihm they specify generates 64-bit hashes
			foreach(var entry in Entries) {
				if (hashA == (ulong)entry.FilePathHashA && hashB == (ulong)entry.FilePathHashB)
					return entry;
			}

			return null;
		}
	}
}