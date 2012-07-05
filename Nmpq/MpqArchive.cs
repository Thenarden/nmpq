using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Nmpq.Parsing;

namespace Nmpq {
	public class MpqArchive : IMpqArchive, IDisposable {
		private BinaryReader _reader;
		private bool _cleanupStreamOnDispose;

		public int ArchiveOffset { get; set; }
		public int UserDataSize { get; set; }
		public MpqHeader Header { get; set; }
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

			var hashTableEntries = ReadTableEntires<HashTableEntry>("(hash table)", Header.HashTableOffset, Header.HashTableEntryCount);
			var blockTableEntries = ReadTableEntires<BlockTableEntry>("(block table)", Header.BlockTableOffset, Header.BlockTableEntryCount);

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

			Header = _reader.ReadStruct<MpqHeader>();

			if (!Header.IsMagicValid)
				throw new Exception("Invalid MPQ header, this is probably not an MPQ archive. (Invalid magic)");

			if (!Header.IsBurningCrusadeFormat)
				throw new Exception("Invalid MPQ format. Must be '1'.");

			if (Header.HeaderSize != 0x2c)
				throw new Exception("Unexpected header size for specified MPQ format.");
		}

		private IEnumerable<T> ReadTableEntires<T>(string name, int offset, int numberOfEntries) {
			Seek(offset);

			var size = Marshal.SizeOf(typeof(T));
			var data = _reader.ReadBytes(size * numberOfEntries);
			var key = Crypto.Hash(name, HashType.TableOffset);

			Crypto.DecryptInPlace(data, key);

			using (var memoryStream = new MemoryStream(data))
			using (var tableReader = new BinaryReader(memoryStream)) {
				while (tableReader.BaseStream.Position != tableReader.BaseStream.Length)
					yield return tableReader.ReadStruct<T>();
			}
		}

		private static ulong ComputeFileKey(string path, BlockTableEntry blockTableEntry, ulong archiveOffset) {
			if (path == null) throw new ArgumentNullException("path");

			var filename = Path.GetFileName(path); // is this kosher? do MPQ paths have the same syntax and semantics as system paths?
			var fileKey = Crypto.Hash(filename, HashType.FileKey);

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
		public byte[] OpenFile(string path) {
			if (path == null) throw new ArgumentNullException("path");

			var hashA = Crypto.Hash(path, HashType.FilePathA);
			var hashB = Crypto.Hash(path, HashType.FilePathB);

			var entry = HashTable.FindEntry(hashA, hashB);

			if (entry == null)
				return null;

			var blockEntry = BlockTable[entry.Value.FileBlockIndex];

			if (!blockEntry.IsFile)
				return null;

			Seek(blockEntry.BlockOffset);

			var data = _reader.ReadBytes(blockEntry.BlockSize);

			if (blockEntry.IsFileSingleUnit) {
				if (blockEntry.IsCompressed)
					return null;

				return data;
			}

			return null;
		}
	}
}
