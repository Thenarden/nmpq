using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
				var archiveOffset = ValidateMagicAndGetArhiveOffset(reader);

				var header = reader.ReadStruct<MpqHeader>();
				ValidateHeader(header);

				var hashTableEntries = ReadTableEntires<HashTableEntry>(reader, "hash", archiveOffset + header.HashTableOffset, header.HashTableEntires);
				var blockTableEntries = ReadTableEntires<BlockTableEntry>(reader, "block", archiveOffset + header.BlockTableOffset, header.BlockTableEntries);

				return new MpqArchive {
					Header = header,
					BlockTable = new BlockTable(blockTableEntries.ToArray()),
					HashTable = new HashTable(hashTableEntries.ToArray()),
				};
			}
		}

		private int ValidateMagicAndGetArhiveOffset(BinaryReader reader) {
			var magicString = new string(reader.ReadChars(3));
			var userDataIndicator = reader.ReadByte();
			
			if (magicString != "MPQ") 
				throw new Exception("Invalid MPQ header. This is probably not an MPQ archive. (Invalid magic)");

			// 0x1a as the last byte of the magic number indicates that there is no user data section
			if (userDataIndicator == 0x1a)
				return 0;

			// 0x1b as the last byte of the magic number indicates that there IS a user data section
			//	we have to skip over it to get to the archive header
			if (userDataIndicator == 0x1b) {
				var userDataSize = reader.ReadInt32();	// don't care about this
				var archiveOffset = reader.ReadInt32();

				return archiveOffset;
			}

			throw new Exception("Invalid MPQ header. This is probably not an MPQ archive. (Invalid user data indicator)");
		}

		private static void ValidateHeader(MpqHeader header) {
			if (!header.IsMagicValid)
				throw new Exception("Invalid MPQ header, this is probably not an MPQ archive. (Invalid magic)");

			if (!header.IsBurningCrusadeFormat)
				throw new Exception("Invalid MPQ format. Must be '1'.");

			if (header.HeaderSize != 0x2c)
				throw new Exception("Unexpected header size for specified MPQ format.");
		}
		
		private static ulong ComputeFileKey(string path, BlockTableEntry blockTableEntry, ulong archiveOffset) {
			if (path == null) throw new ArgumentNullException("path");

			var filename = Path.GetFileName(path); // is this kosher? do MPQ paths have the same syntax and semantics as system paths?
			var fileKey = Crypto.Hash(filename, HashType.FileKey);

			return blockTableEntry.HasKeyAdjustedByBlockOffset
					? (fileKey + (ulong)blockTableEntry.BlockOffset) ^ (ulong)blockTableEntry.FileSize
					: fileKey;
		}

		private static IEnumerable<T> ReadTableEntires<T>(BinaryReader reader, string name, int offset, int numberOfEntries) {
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);

			var size = Marshal.SizeOf(typeof(T));
			var data = reader.ReadBytes(size * numberOfEntries);
			var key = Crypto.Hash("(" + name + " table)", HashType.TableOffset);

			Crypto.DecryptInPlace(data, key);

			using (var memoryStream = new MemoryStream(data))
			using (var tableReader = new BinaryReader(memoryStream)) {
				while (tableReader.BaseStream.Position != tableReader.BaseStream.Length)
					yield return tableReader.ReadStruct<T>();
			}
		} 
	}
}