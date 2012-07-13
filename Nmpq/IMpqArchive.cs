using System.Collections.Generic;

namespace Nmpq {
	public interface IMpqArchive {
		IList<string> KnownFiles { get; }

		byte[] ReadFile(string path);
		object ReadSerializedData(string path, bool convertStringsToUtf8);
	}
}