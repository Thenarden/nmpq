using Nmpq.Parsing;

namespace Nmpq {
	public class MpqArchive {
		public MpqHeader Header { get; set; }
		public HashTable HashTable { get; set; }
		public BlockTable BlockTable { get; set; }
	}
}
