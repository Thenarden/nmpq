using System.Runtime.InteropServices;

namespace Nmpq.Parsing {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MpqHeader {
		public bool IsMagicValid {
			get {
				return Magic[0] == (byte) 'M'
					&& Magic[1] == (byte) 'P'
					&& Magic[2] == (byte) 'Q'
					&& Magic[3] == 0x1a; // docs say this is supposed to be 0x1a, but my tests are showing 0x1b?
			}
		}

		public bool IsBurningCrusadeFormat {
			get { return FormatVersion == 1; }
		}

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public byte[] Magic;

		public int HeaderSize;
		public int ArchiveSize;
		public short FormatVersion;
		public byte SectorSizeShift;
		public int HashTableOffset;
		public int BlockTableOffset;
		public int HashTableEntires;
		public int BlockTableEntries;

		public long ExtendedBlockTableOffset;
		public short HashTableOffsetHigh;
		public short BlockTableOffsetHigh;
	}

	public static class Hashing {
		private static readonly ulong[] CryptTable = new ulong[0x500];

		static Hashing() {
			InitializeCryptTable();
		}

		public static ulong Hash(string lpszString, ulong dwHashType) {
			ulong seed1 = 0x7FED7FEDL;
			ulong seed2 = 0xEEEEEEEEL;

			foreach (var c in lpszString.ToUpperInvariant()) {
				ulong ch = (byte) c;
				seed1 = CryptTable[(dwHashType*0x100) + ch] ^ (seed1 + seed2);
				seed2 = ch + seed1 + seed2 + (seed2 << 5) + 3;
			}

			return seed1;
		}

		// The encryption and hashing functions use a number table in their procedures. This table must be initialized before the functions are called the first time.
		private static void InitializeCryptTable() {
			ulong seed = 0x00100001;

			for (ulong index1 = 0; index1 < 0x100; index1++) {
				var i = 0;

				for (ulong index2 = index1; i < 5; i++, index2 += 0x100) {
					seed = (seed*125 + 3)%0x2AAAAB;
					ulong temp1 = (seed & 0xFFFF) << 0x10;

					seed = (seed*125 + 3)%0x2AAAAB;
					ulong temp2 = (seed & 0xFFFF);

					CryptTable[index2] = (temp1 | temp2);
				}
			}
		}
	}
}