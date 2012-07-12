using System;
using System.IO;

namespace Nmpq.Parsing {
	public class MpqFileStream : Stream {
		private readonly BlockTableEntry _blockTableEntry;
		private Stream _decompressor;

		public MpqFileStream(BlockTableEntry blockTableEntry) {
			_blockTableEntry = blockTableEntry;
		}

		public override void Flush() {
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin) {
			throw new System.NotImplementedException();
		}

		public override void SetLength(long value) {
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count) {
			throw new System.NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count) {
			throw new NotSupportedException();
		}

		public override bool CanRead {
			get { return true; }
		}

		public override bool CanSeek {
			get { return false; }
		}

		public override bool CanWrite {
			get { return false; }
		}

		private long _length;
		private long _position;

		public override long Length {
			get { return _length; }
		}

		public override long Position {
			get { return _position; }
			set { throw new NotSupportedException(); }
		}
	}
}