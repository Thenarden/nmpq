namespace Nmpq {
	public interface IMpqUserDataHeader {
		int ArchiveOffset { get; }
		byte[] UserData { get; }
		int UserDataReservedSize { get; }
		int UserDataSize { get; }
	}
}