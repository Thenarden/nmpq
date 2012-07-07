using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;
using Nmpq.Parsing;

namespace Nmpq {
	public class MpqArchive : IMpqArchive, IDisposable {
		private BinaryReader _reader;
		private bool _cleanupStreamOnDispose;

		public int ArchiveOffset { get; set; }
		public int UserDataSize { get; set; }
		public ArchiveHeader ArchiveHeader { get; set; }
		public HashTable HashTable { get; set; }
		public BlockTableEntry[] BlockTable { get; set; }

		protected MpqArchive() {
		}

		public static MpqArchive Open(string path) {
			if (path == null) throw new ArgumentNullException("path");

			var archive = new MpqArchive();
			archive.OpenInternal(File.OpenRead(path), true);
			return archive;
		}

		public static MpqArchive Open(byte[] data) {
			if (data == null) throw new ArgumentNullException("data");

			var archive = new MpqArchive();
			archive.OpenInternal(new MemoryStream(data), true);
			return archive;
		}

		public static MpqArchive Open(Stream stream) {
			if (stream == null) throw new ArgumentNullException("stream");

			var archive = new MpqArchive();
			archive.OpenInternal(stream, false);
			return archive;
		}

		private void OpenInternal(Stream stream, bool cleanupStreamOnDispose) {
			_reader = new BinaryReader(stream, Encoding.ASCII);
			_cleanupStreamOnDispose = cleanupStreamOnDispose;

			ReadUserDataHeader();
			ReadAndValidateArchiveHeader();

			var hashTableEntries = ReadTableEntires<HashTableEntry>("(hash table)", ArchiveHeader.HashTableOffset, ArchiveHeader.HashTableEntryCount);
			var blockTableEntries = ReadTableEntires<BlockTableEntry>("(block table)", ArchiveHeader.BlockTableOffset, ArchiveHeader.BlockTableEntryCount);

			BlockTable = blockTableEntries.ToArray();
			HashTable = new HashTable(hashTableEntries.ToArray());
		}

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

				yield return (T)Marshal.PtrToStructure(entryPointer, typeof (T));
			}

			handle.Free();

			//using (var memoryStream = new MemoryStream(data))
			//using (var tableReader = new BinaryReader(memoryStream)) {
			//	while (tableReader.BaseStream.Position != tableReader.BaseStream.Length)
			//		yield return tableReader.ReadStruct<T>();
			//}
		}

		private static ulong ComputeFileKey(string path, BlockTableEntry blockTableEntry, ulong archiveOffset) {
			if (path == null) throw new ArgumentNullException("path");

			var filename = Path.GetFileName(path); // is this kosher? do MPQ paths have the same syntax and semantics as system paths?
			var fileKey = Crypto.Hash(filename, HashType.TableKey);

			return blockTableEntry.HasKeyAdjustedByBlockOffset
					? (fileKey + (ulong)blockTableEntry.BlockOffset) ^ (ulong)blockTableEntry.FileSize
					: fileKey;
		}

		// seeks to a stream posistion relative to the start of the archive
		private void Seek(long offset) {
			_reader.BaseStream.Seek(ArchiveOffset + offset, SeekOrigin.Begin);
		}

		public void Dispose() {
			if (_cleanupStreamOnDispose) {
				if (_reader != null) {
					_reader.Dispose();
					_reader = null;
				}
			}
		}

		// todos: decompression, multi-block files
		public byte[] ReadFileBytes(string path) {
			if (path == null) throw new ArgumentNullException("path");

			var hashA = Crypto.Hash(path, HashType.FilePathA);
			var hashB = Crypto.Hash(path, HashType.FilePathB);

			var entry = HashTable.FindEntry(hashA, hashB);

			if (entry == null)
				return null;

			var blockEntry = BlockTable[entry.Value.FileBlockIndex];

			if (!blockEntry.IsFile)
				throw new NotSupportedException("Non-file blocks are not currently supported by Nmpq.");

			if (blockEntry.IsEncrypted)
				throw new NotSupportedException("Encrypted files are not currently supported by Nmpq.");

			if (blockEntry.IsImploded)
				throw new NotSupportedException("Imploded files are not currently supported by Nmpq.");

			if (!blockEntry.IsFileSingleUnit)
				throw new NotSupportedException("Multi-block files are not currently supported by Nmpq.");

			Seek(blockEntry.BlockOffset);

			var block = _reader.ReadBytes(blockEntry.BlockSize);

			if (blockEntry.IsCompressed) {
				var compressionFlag = (CompressionFlags) block[0];

				if (compressionFlag != CompressionFlags.Bzip2)
					throw new NotSupportedException("Currenlty only Bzip2 compression is supported by Nmpq.");

				using(var inputStream = new MemoryStream(block, 1, block.Length - 1))	// skip the first byte, which is the compression flag
				using(var outputStream = new MemoryStream()) {
					BZip2.Decompress(inputStream, outputStream, false);
					return outputStream.ToArray();
				}
			}
			
			return block;
		}
	}
}
