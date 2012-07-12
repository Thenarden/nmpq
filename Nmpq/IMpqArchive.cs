using System.Collections.Generic;
using System.IO;

namespace Nmpq {
	public interface IMpqArchive {
		IList<string> KnownFiles { get; }

		Stream OpenFile(string path);
		byte[] ExtractFileBytes(string path);
		object ExtractSerializedData(string path, bool convertStringsToUtf8);
	}
}