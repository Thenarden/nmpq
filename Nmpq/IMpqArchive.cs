using System.IO;

namespace Nmpq {
	public interface IMpqArchive {
		byte[] ReadFileBytes(string path);
	}
}