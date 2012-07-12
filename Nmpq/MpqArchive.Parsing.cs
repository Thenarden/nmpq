using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;
using Nmpq.Parsing;

namespace Nmpq {
	public partial class MpqArchive {
		public bool HasOpenFile { get; private set; }

		private void ReadUserDataHeader() {
			var magicString = new string(_reader.ReadChars(3));
			var userDataIndicator = _reader.ReadByte();

			if (magicString != "MPQ" || (userDataIndicator != 0x1a && userDataIndicator != 0x1b))
				throw new Exception("Invalid MPQ header. This is probably not an MPQ archive. (Invalid magic)");

			// 0x1a as the last byte of the magic number indicates that there is no user data section
			if (userDataIndicator == 0x1a) {
				ArchiveOffset = 0;
				UserDataSize = 0;
			}

			// 0x1b as the last byte of the magic number indicates that there IS a user data section
			//	we have to skip over it to get to the archive header
			if (userDataIndicator == 0x1b) {
				UserDataSize = _reader.ReadInt32();
				ArchiveOffset = _reader.ReadInt32();
			}
		}

		private void ReadAndValidateArchiveHeader() {
			Seek(0);

			ArchiveHeader = _reader.ReadStruct<ArchiveHeader>();

			if (!ArchiveHeader.IsMagicValid)
				throw new Exception("Invalid MPQ header, this is probably not an MPQ archive. (Invalid magic)");

			if (!ArchiveHeader.IsBurningCrusadeFormat)
				throw new Exception("Invalid MPQ format. Must be '1'.");

			if (ArchiveHeader.HeaderSize != 0x2c)
				throw new Exception("Unexpected header size for specified MPQ format.");
		}

		private IEnumerable<T> ReadTableEntires<T>(string name, int tableOffset, int numberOfEntries) {
			Seek(tableOffset);

			var entrySize = Marshal.SizeOf(typeof(T));
			var data = _reader.ReadBytes(entrySize * numberOfEntries);
			var key = Crypto.Hash(name, HashType.TableKey);

			Crypto.DecryptInPlace(data, key);

			var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			var addr = handle.AddrOfPinnedObject();

			for (var i = 0; i < numberOfEntries; i++) {
				var entryOffset = i * entrySize;
				var entryPointer = addr + entryOffset;

				yield return (T)Marshal.PtrToStructure(entryPointer, typeof(T));
			}

			handle.Free();
		}

		private BlockTableEntry? FindBlockTableEntry(string path) {
			var hashA = Crypto.Hash(path, HashType.FilePathA);
			var hashB = Crypto.Hash(path, HashType.FilePathB);

			var entry = HashTable.FindEntry(hashA, hashB);

			if (entry == null)
				return null;

			return BlockTable[entry.Value.FileBlockIndex];
		}

		// todos: better decompression, multi-block files
		public byte[] ExtractFileBytes(string path) {
			if (path == null) throw new ArgumentNullException("path");

			var blockEntry = FindBlockTableEntry(path);

			if (blockEntry == null)
				return null;

			if (!blockEntry.Value.IsFile)
				throw new NotSupportedException("Non-file blocks are not currently supported by Nmpq.");

			if (blockEntry.Value.IsEncrypted)
				throw new NotSupportedException("Encrypted files are not currently supported by Nmpq.");

			if (blockEntry.Value.IsImploded)
				throw new NotSupportedException("Imploded files are not currently supported by Nmpq.");

			if (!blockEntry.Value.IsFileSingleUnit)
				throw new NotSupportedException("Multi-block files are not currently supported by Nmpq.");

			Seek(blockEntry.Value.BlockOffset);

			// file is only compressed if the block size is smaller than the file size.
			//	per docs at (http://wiki.devklog.net/index.php?title=MPQ_format_specification)
			var compressed = blockEntry.Value.IsCompressed && blockEntry.Value.BlockSize < blockEntry.Value.FileSize;

			if (!compressed) {
				return _reader.ReadBytes(blockEntry.Value.BlockSize);
			}

			// first byte of each compressed block is a set of flags indicating which 
			//	compression algorithm(s) to use
			var compressionFlags = (CompressionFlags) _reader.ReadByte();

			// compression flags don't count toward the data size, but does toward the block size
			var dataSize = blockEntry.Value.BlockSize - 1; 
			var blockData = _reader.ReadBytes(dataSize);

			if (compressionFlags == CompressionFlags.Bzip2) {
				using (var inputStream = new MemoryStream(blockData))
				using (var outputStream = new MemoryStream()) {
					BZip2.Decompress(inputStream, outputStream, false);
					return outputStream.ToArray();
				}
			}

			if (compressionFlags == CompressionFlags.Deflated) {
				// see http://george.chiramattel.com/blog/2007/09/deflatestream-block-length-does-not-match.html
				// and possibly http://connect.microsoft.com/VisualStudio/feedback/details/97064/deflatestream-throws-exception-when-inflating-pdf-streams
				// for more info on why we have to skip two extra bytes because of ZLIB
				using (var inputStream = new MemoryStream(blockData, 2, blockData.Length - 2)) // skip ZLIB bytes 
				using (var deflate = new DeflateStream(inputStream, CompressionMode.Decompress)) 
				using (var outputStream = new MemoryStream()) {
					var buffer = new byte[1024];
					var read = deflate.Read(buffer, 0, buffer.Length);

					while(read == buffer.Length) {
						outputStream.Write(buffer, 0, read);
						read = deflate.Read(buffer, 0, buffer.Length);
					}

					outputStream.Write(buffer, 0, read);
					return outputStream.ToArray();
				}
			}

			throw new NotSupportedException("Currenlty only Bzip2 and Deflate compression is supported by Nmpq.");
		}

		private List<string> ParseListfile() {
			var listfile = ExtractFileBytes("(listfile)");

			if (listfile == null) {
				return new List<string>();
			}

			var contents = Encoding.UTF8.GetString(listfile);
			var entries = contents.Split(new[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			return entries.ToList();
		}


		public object ExtractSerializedData(string path, bool convertStringsToUtf8) {
			using(var memory = new MemoryStream(ExtractFileBytes(path))) 
			using(var reader = new BinaryReader(memory)) {
				return Deserialization.ParseSerializedData(reader, convertStringsToUtf8);
			}
		}

		public Stream OpenFile(string path) {
			if (path == null) throw new ArgumentNullException("path");

			if (HasOpenFile)
				throw new InvalidOperationException(
					"There is already a file open for this archive. You can only have one archive file stream open at a time.");

			var blockEntry = FindBlockTableEntry(path);

			if (blockEntry == null)
				return null;

			HasOpenFile = true;
			var fileOffset = blockEntry.Value.BlockOffset + ArchiveOffset;
			return new MpqFileStream(blockEntry.Value, _reader.BaseStream, fileOffset, () => { HasOpenFile = false; });
		}
	}
}
