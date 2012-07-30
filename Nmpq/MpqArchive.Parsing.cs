using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Nmpq.Parsing;

namespace Nmpq {
	public partial class MpqArchive {
		private static MpqUserDataHeader ParseUserDataHeader(BinaryReader reader) {
			var header = new MpqUserDataHeader();

			var magicString = new string(reader.ReadChars(3));
			var userDataIndicator = reader.ReadByte();

			if (magicString != "MPQ" || (userDataIndicator != 0x1a && userDataIndicator != 0x1b))
				return null;

			// 0x1a as the last byte of the magic number indicates that there is no user data section
			if (userDataIndicator == 0x1a) {
				header.HasUserData = false;

				header.ArchiveOffset = 0;
				header.UserDataReservedSize = 0;
			}

			// 0x1b as the last byte of the magic number indicates that there IS a user data section
			//	we have to skip over it to get to the archive header
			if (userDataIndicator == 0x1b) {
				header.HasUserData = true;

				header.UserDataReservedSize = reader.ReadInt32();
				header.ArchiveOffset = reader.ReadInt32();

				header.UserDataSize = reader.ReadInt32();
				header.UserData = reader.ReadBytes(header.UserDataSize);
			}

			return header;
		}

		private void ParseArchiveHeader() {
			SeekToArchiveOffset(0);

			ArchiveHeader = _reader.ReadStruct<ArchiveHeader>();

			if (!ArchiveHeader.IsMagicValid)
				throw new MpqParsingException("Invalid MPQ header, this is probably not an MPQ archive. (Invalid magic)");

			if (!ArchiveHeader.IsBurningCrusadeFormat)
				throw new MpqParsingException("Invalid MPQ format. Must be '1'.");

			if (ArchiveHeader.HeaderSize != 0x2c)
				throw new MpqParsingException("Unexpected header size for specified MPQ format.");
		}

		private IEnumerable<T> ReadTableEntires<T>(string name, int tableOffset, int numberOfEntries) {
			SeekToArchiveOffset(tableOffset);

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

		// todos: support more decompression algorithms?
		public byte[] ReadFile(string path) {
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

			SeekToArchiveOffset(blockEntry.Value.BlockOffset);

			if (!blockEntry.Value.IsFileSingleUnit) {
				return ReadMultiUnitFile(blockEntry.Value);
			}

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
				return Compression.BZip2Decompress(blockData, 0);
			}

			if (compressionFlags == CompressionFlags.Deflated) {
				return Compression.Deflate(blockData, 0);
			}

			throw new NotSupportedException("Currenlty only Bzip2 and Deflate compression is supported by Nmpq.");
		}

		private byte[] ReadMultiUnitFile(BlockTableEntry blockEntry) {
			var sectorCount = blockEntry.FileSize/SectorSize;

			if (blockEntry.FileSize % SectorSize > 0)
				sectorCount++;

			var sectorTable = new int[sectorCount + 1];

			for(var i = 0; i < sectorTable.Length; i++) {
				sectorTable[i] = _reader.ReadInt32();
			}

			var result = new byte[blockEntry.FileSize];
			var resultPosition = 0;

			for(var i = 0; i < sectorCount; i++) {
				var position = sectorTable[i];
				var length = sectorTable[i + 1] - position;

				SeekToArchiveOffset(position + blockEntry.BlockOffset);
				var sectorData = _reader.ReadBytes(length);

				if (blockEntry.IsCompressed && length < SectorSize) {
					var compressionFlags = (CompressionFlags) sectorData[0];

					if (compressionFlags == CompressionFlags.Bzip2) {
						sectorData = Compression.BZip2Decompress(sectorData, 1);
					}
					else if(compressionFlags == CompressionFlags.Deflated) {
						sectorData = Compression.Deflate(sectorData, 1);
					}
					else {
						throw new NotSupportedException("Currenlty only Bzip2 and Deflate compression is supported by Nmpq.");
					}
				}

				Array.ConstrainedCopy(sectorData, 0, result, resultPosition, sectorData.Length);
				resultPosition += sectorData.Length;
			}

			return result;
		}

		private List<string> ParseListfile() {
			var listfile = ReadFile("(listfile)");

			if (listfile == null) {
				return new List<string>();
			}

			var contents = Encoding.UTF8.GetString(listfile);
			var entries = contents.Split(new[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			return entries.ToList();
		}


		public object ReadSerializedData(string path, bool convertStringsToUtf8) {
			if (path == null) throw new ArgumentNullException("path");

			var file = ReadFile(path);
			
			if (file == null)
				return null;

			using(var memory = new MemoryStream(file)) 
			using(var reader = new BinaryReader(memory)) {
				return MpqSerializedData.Deserialize(reader, convertStringsToUtf8);
			}
		}
	}
}
