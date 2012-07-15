using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nmpq.Parsing;

namespace Nmpq {
	public partial class MpqArchive : IMpqArchive, IDisposable {
		public int ArchiveOffset { get; set; }

		public int UserDataMaxSize { get; set; }
		public byte[] UserDataHeader { get; set; }

		// I am not sure what this field actually means, but my MPQ editor 
		//		says it exists at offset 0x0c in the file. That also matches
		//		what I am seeing elsewhere, though no reference appears in
		//		the canonical online MPQ docs here: http://wiki.devklog.net/index.php?title=MPQ_format_specification#User_Data
		public int UserDataHeaderSize { get; set; } 

		public ArchiveHeader ArchiveHeader { get; set; }
		public HashTable HashTable { get; set; }
		public BlockTableEntry[] BlockTable { get; set; }
		public int SectorSize { get; set; }


		private BinaryReader _reader;
		private bool _cleanupStreamOnDispose;
		private IList<string> _knownFiles;

		public IList<string> KnownFiles {
			get {
				if (_knownFiles == null) {
					_knownFiles = ParseListfile().AsReadOnly();
				}

				return _knownFiles;
			}
		}

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
			_reader = new BinaryReader(stream, Encoding.UTF8);
			_cleanupStreamOnDispose = cleanupStreamOnDispose;

			ReadUserDataHeader();
			ReadAndValidateArchiveHeader();

			SectorSize = 512 << ArchiveHeader.SectorSizeShift;

			var hashTableEntries = ReadTableEntires<HashTableEntry>("(hash table)", ArchiveHeader.HashTableOffset, ArchiveHeader.HashTableEntryCount);
			var blockTableEntries = ReadTableEntires<BlockTableEntry>("(block table)", ArchiveHeader.BlockTableOffset, ArchiveHeader.BlockTableEntryCount);

			BlockTable = blockTableEntries.ToArray();
			HashTable = new HashTable(hashTableEntries.ToArray());
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
	}
}
