namespace WC3LanGame.Warcraft3
{
    internal static class ByteExtensions
    {
        public static byte[] ToBytes(this ushort number, bool littleEndianOrder = true)
        {
            byte[] bytes = BitConverter.GetBytes(number);
            if ((littleEndianOrder && !BitConverter.IsLittleEndian) || (!littleEndianOrder && BitConverter.IsLittleEndian))
                Array.Reverse(bytes);

            return bytes;
        }

        public static byte[] ToBytes(this uint number, bool littleEndianOrder = true)
        {
            byte[] bytes = BitConverter.GetBytes(number);
            if ((littleEndianOrder && !BitConverter.IsLittleEndian) || (!littleEndianOrder && BitConverter.IsLittleEndian))
                Array.Reverse(bytes);

            return bytes;
        }
    }
}
