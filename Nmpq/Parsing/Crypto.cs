using System;

namespace Nmpq.Parsing {
	public static class Crypto {
		private static readonly ulong[] CryptTable = new ulong[0x500];

		static Crypto() {
			InitializeCryptTable();
		}

		public static void DecryptInPlace(byte[] data, ulong key) {
			if (data == null) throw new ArgumentNullException("data");

			ulong seed = 0xEEEEEEEEL;

			for(var i = 0; i < data.Length; i += sizeof(ulong)) {
				var current = BitConverter.ToUInt64(data, i);
				seed += CryptTable[0x400 + (key & 0xff)];

				var decrypted = current ^ (key + seed);
				var bytes = BitConverter.GetBytes(decrypted);
				Array.ConstrainedCopy(bytes, 0, data, i, sizeof(ulong));

				key = ((~key << 0x15) + 0x11111111) | (key >> 0x0b);
				seed = current + seed + (seed << 5) + 3;
			}
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