namespace Nmpq.Parsing {
	public enum SerializedDataType : byte {
		String = 2,
		Array = 4,
		Map = 5,
		SingleByteInteger = 6,
		FourByteInteger = 7,
		VariableLengthInteger = 9,
	}
}