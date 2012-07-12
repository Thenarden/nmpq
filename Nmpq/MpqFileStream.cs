using System;
using System.IO;
using Nmpq.Parsing;

namespace Nmpq {
	public class MpqFileStream : Stream {
		private readonly BlockTableEntry _blockTableEntry;
		private readonly Stream _file;
		private readonly long _fileOffset;
		private readonly Action _onClose;

		private long _position;
		private long _length;

		private bool _compressed;
		private Stream _decompressor;

		public MpqFileStream(BlockTableEntry blockTableEntry, Stream file, long fileOffset,  Action onClose) {
			_blockTableEntry = blockTableEntry;
			_file = file;
			_fileOffset = fileOffset;
			_onClose = onClose;

			_position = 0;
			_length = _blockTableEntry.FileSize;

			if (!_blockTableEntry.IsFileSingleUnit)
				throw new NotSupportedException("file must be single unit");

			_file.Seek(_fileOffset, SeekOrigin.Begin);
			_compressed = _blockTableEntry.IsCompressed && _blockTableEntry.BlockSize < _blockTableEntry.FileSize;
		}

		public override long Seek(long offset, SeekOrigin origin) {
			var newPosition = 0L;

			if (origin == SeekOrigin.Begin)
				newPosition = offset;

			if (origin == SeekOrigin.Current)
				newPosition = _position + offset;

			if (origin == SeekOrigin.End)
				newPosition = _length - offset;

			if (newPosition < 0)
				throw new InvalidOperationException("Cannot seek past beginning of file in archive.");

			if (newPosition > _length)
				throw new InvalidOperationException("Cannot seek past end of file in archive.");

			_file.Seek(_fileOffset + offset, origin);
			return newPosition;
		}

		public override int Read(byte[] buffer, int offset, int count) {
			if (count + _position > _length)
				count = (int)(_length - _position);

			var read = _file.Read(buffer, offset, count);
			_position += count;
			return read;
		}

		public override bool CanRead {
			get { return true; }
		}

		public override bool CanSeek {
			get { return true; }
		}

		public override bool CanWrite {
			get { return false; }
		}

		public override long Length {
			get { return _length; }
		}

		public override long Position {
			get { return _position; }
			set { throw new NotSupportedException(); }
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);

			if (_onClose != null)
				_onClose();
		}
	
		public override void SetLength(long value) {
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count) {
			throw new NotSupportedException();
		}

		public override void Flush() {
			throw new NotSupportedException();
		}
	}
}