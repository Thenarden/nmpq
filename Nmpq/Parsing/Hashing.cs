namespace Nmpq.Parsing {
	public static class Hashing {
		private static readonly ulong[] CryptTable = new ulong[0x500];

		static Hashing() {
			InitializeCryptTable();
		}

		public static ulong Hash(string str, HashType hashType) {
			ulong seed1 = 0x7FED7FEDL;
			ulong seed2 = 0xEEEEEEEEL;

			foreach (var c in str.ToUpperInvariant()) {
				ulong ch = (byte) c;
				seed1 = CryptTable[((ulong)hashType*0x100) + ch] ^ (seed1 + seed2);
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