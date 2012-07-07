using System.Collections.Generic;

namespace Nmpq {
	public interface IMpqArchive {
		IList<string> KnownFiles { get; }

		byte[] ReadFileBytes(string path);
	}
}