namespace Nmpq {
	class MpqUserDataHeader : IMpqUserDataHeader {
		public bool HasUserData { get; set; }
		public int ArchiveOffset { get; set; }
		public byte[] UserData { get; set; }
		public int UserDataReservedSize { get; set; }
		public int UserDataSize { get; set; }
	}
}