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

			using(var reader = new BinaryReader(archive, Encoding.ASCII)) {
				ReadMagicAndSkipToArchiveHeader(reader);

				var header = reader.ReadStruct<MpqHeader>();
				ValidateHeader(header);

				return new MpqArchive { Header = header };
			}
		}

		private void ReadMagicAndSkipToArchiveHeader(BinaryReader reader) {
			var magicString = new string(reader.ReadChars(3));
			var userDataIndicator = reader.ReadByte();
			
			if (magicString != "MPQ" || (userDataIndicator != 0x1a && userDataIndicator != 0x1b))
				throw new Exception("Invalid MPQ header. This is probably not an MPQ archive. (Invalid magic)");

			// 0x1a as the last byte of the magic number indicates that there is no user data section
			if (userDataIndicator == 0x1a)
				return;

			// 0x1b as the last byte of the magic number indicates that there IS a user data section
			//	we have to skip over it to get to the archive header
			if (userDataIndicator == 0x1b) {
				var userDataSize = reader.ReadInt32();	// don't care about this
				var archiveOffset = reader.ReadInt32();

				// todo: consider whether it is worth rewriting this to handle un-seekable streams
				reader.BaseStream.Seek(archiveOffset, SeekOrigin.Begin);
			}
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