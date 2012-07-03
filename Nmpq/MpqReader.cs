using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Nmpq.Parsing;

namespace Nmpq {
	public class MpqReader {
		public MpqArchive ReadArchive(string path) {
			if (path == null) throw new ArgumentNullException("path");

			using (var archive = File.OpenRead(path)) {
				return ReadArchive(archive);
			}
		}

		public MpqArchive ReadArchive(byte[] archive) {
			if (archive == null) throw new ArgumentNullException("archive");

			using (var stream = new MemoryStream(archive)) {
				return ReadArchive(stream);
			}
		}

		public MpqArchive ReadArchive(Stream archive) {
			if (archive == null) throw new ArgumentNullException("archive");

			var reader = new BinaryReader(archive, Encoding.ASCII);
			
			var header = reader.ReadStruct<MpqHeader>();
			ValidateHeader(header);

			return new MpqArchive { Header = header };
		}

		private static void ValidateHeader(MpqHeader header) {
			if (!header.IsMagicValid)
				throw new Exception("Invalid MPQ header, this is probably not an MPQ archive. (Invalid magic)");

			if (!header.IsBurningCrusadeFormat)
				throw new Exception("Invalid MPQ format. Must be '1'.");

			if (header.HeaderSize != 0x2c)
				throw new Exception("Unexpected header size for specified MPQ format.");
		}
	}
}