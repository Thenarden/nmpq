using NUnit.Framework;
using Nmpq.Parsing;

namespace Nmpq.Tests {
	[TestFixture]
	public class CryptoTests {
		[Test]
		public void FilePathA_hash_is_calculated_correctly() {
			var hashA = Crypto.Hash("(listfile)", HashType.FilePathA);

			Assert.That(hashA, Is.EqualTo(0xfd657910));
		}

		[Test]
		public void FilePathB_hash_is_calculated_correctly() {
			var hashA = Crypto.Hash("(listfile)", HashType.FilePathB);

			Assert.That(hashA, Is.EqualTo(0x4e9b98a7));
		}
	}
}