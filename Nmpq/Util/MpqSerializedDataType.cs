namespace Nmpq.Util
{
    public enum MpqSerializedDataType : byte
    {
        BinaryString = 2,
        Array = 4,
        Map = 5,
        SingleByteInteger = 6,
        FourByteInteger = 7,
        VariableLengthInteger = 9,
    }
}