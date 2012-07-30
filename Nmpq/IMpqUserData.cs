namespace Nmpq {
	public interface IMpqUserData {
		byte[] UserData { get; }
		int UserDataReservedSize { get; }
		int UserDataSize { get; }
	}
}