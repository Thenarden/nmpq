using System.IO;

namespace Nmpq {
	public interface IMpqArchive {
		byte[] OpenFile(string path);
	}
}