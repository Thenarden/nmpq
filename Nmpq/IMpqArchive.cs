using System;
using System.Collections.Generic;

namespace Nmpq {
	public interface IMpqArchive : IDisposable {
		IList<string> KnownFiles { get; }

		byte[] ReadFile(string path);
		object ReadSerializedData(string path, bool convertStringsToUtf8);

		IMpqArchiveDetails Details { get; }
		IMpqUserDataHeader UserDataHeader { get; }
	}
}