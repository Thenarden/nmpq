using Nmpq.Parsing;

namespace Nmpq {
	public class HashTable {
		public HashTable(HashTableEntry[] entries) {
			Entries = entries;
		}

		public HashTableEntry[] Entries { get; private set; }

		public HashTableEntry? FindEntry(ulong hashA, ulong hashB) {
			foreach(var entry in Entries) {
				if (hashA == (ulong)entry.FilePathHashA && hashB == (ulong)entry.FilePathHashB)
					return entry;
			}

			return null;
		}
	}
}