using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nmpq.Parsing;

namespace Nmpq
{
    public partial class MpqArchive : IMpqArchive, IMpqArchiveDetails
    {
        public IMpqArchiveDetails Details
        {
            get { return this; }
        }

        public int SectorSize { get; private set; }

        public IMpqUserDataHeader UserDataHeader { get; private set; }
        public ArchiveHeader ArchiveHeader { get; private set; }
        public MpqHashTable HashTable { get; private set; }
        public IList<BlockTableEntry> BlockTable { get; private set; }

        public IList<string> KnownFiles
        {
            get { return _knownFiles ?? (_knownFiles = ParseListfile().AsReadOnly()); }
        }

        private BinaryReader _reader;
        private bool _cleanupStreamOnDispose;
        private IList<string> _knownFiles;

        protected MpqArchive()
        {
        }

        public static IMpqUserDataHeader ParseUserDataHeader(string path)
        {
            if (path == null) throw new ArgumentNullException("path");

            using (var stream = File.OpenRead(path))
                return ParseUserDataHeader(stream);
        }

        public static IMpqUserDataHeader ParseUserDataHeader(byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data");

            using (var stream = new MemoryStream(data))
                return ParseUserDataHeader(stream);
        }

        public static IMpqUserDataHeader ParseUserDataHeader(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            using (var reader = new BinaryReader(stream))
                return ParseUserDataHeader(reader);
        }

        public static IMpqArchive Open(string path)
        {
            if (path == null) throw new ArgumentNullException("path");

            var archive = new MpqArchive();
            archive.OpenInternal(File.OpenRead(path), true);
            return archive;
        }

        public static IMpqArchive Open(byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data");

            var archive = new MpqArchive();
            archive.OpenInternal(new MemoryStream(data), true);
            return archive;
        }

        public static IMpqArchive Open(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            var archive = new MpqArchive();
            archive.OpenInternal(stream, false);
            return archive;
        }

        private void OpenInternal(Stream stream, bool cleanupStreamOnDispose)
        {
            _reader = new BinaryReader(stream, Encoding.UTF8);
            _cleanupStreamOnDispose = cleanupStreamOnDispose;

            UserDataHeader = ParseUserDataHeader(_reader);

            if (UserDataHeader == null)
                throw new MpqParsingException("Invalid MPQ header. This is probably not an MPQ archive.");

            ParseArchiveHeader();

            SectorSize = 512 << ArchiveHeader.SectorSizeShift;

            var hashTableEntries = ReadTableEntires<HashTableEntry>("(hash table)", ArchiveHeader.HashTableOffset,
                                                                    ArchiveHeader.HashTableEntryCount);
            var blockTableEntries = ReadTableEntires<BlockTableEntry>("(block table)", ArchiveHeader.BlockTableOffset,
                                                                      ArchiveHeader.BlockTableEntryCount);

            BlockTable = blockTableEntries.ToList().AsReadOnly();
            HashTable = new MpqHashTable(hashTableEntries.ToArray());
        }

        // seeks to a stream posistion relative to the start of the archive
        private void SeekToArchiveOffset(long offset)
        {
            _reader.BaseStream.Seek(UserDataHeader.ArchiveOffset + offset, SeekOrigin.Begin);
        }

        public void Dispose()
        {
            if (_cleanupStreamOnDispose)
            {
                if (_reader != null)
                {
                    _reader.Dispose();
                    _reader = null;
                }
            }
        }
    }
}