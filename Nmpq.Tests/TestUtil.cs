using System;
using System.Linq;

namespace Nmpq.Tests {
	public static class TestUtil {
		public static byte[] ParseBytes(string byteList) {
			if (byteList == null) throw new ArgumentNullException("byteList");

			return byteList
				.Split(new []{' ', '\r', '\n', '\t'}, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => Convert.ToByte(s, 16))
				.ToArray();
		}
	}
}