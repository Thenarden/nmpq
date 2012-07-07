using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Nmpq {
	public interface IMpqArchive {
		IList<string> KnownFiles { get; }

		Stream OpenFile(string path);
		byte[] ExtractFileBytes(string path);
		JObject ExtractSerializedData(string path);
	}
}