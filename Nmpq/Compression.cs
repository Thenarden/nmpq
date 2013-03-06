using System.IO;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.BZip2;

namespace Nmpq
{
    static class Compression
    {
        public static byte[] BZip2Decompress(byte[] input, int skip)
        {
            using (var inputStream = new MemoryStream(input, skip, input.Length - skip))
            using (var outputStream = new MemoryStream())
            {
                BZip2.Decompress(inputStream, outputStream, false);
                return outputStream.ToArray();
            }
        }

        public static byte[] Deflate(byte[] input, int skip)
        {
            // see http://george.chiramattel.com/blog/2007/09/deflatestream-block-length-does-not-match.html
            // and possibly http://connect.microsoft.com/VisualStudio/feedback/details/97064/deflatestream-throws-exception-when-inflating-pdf-streams
            // for more info on why we have to skip two extra bytes because of ZLIB
            using (var inputStream = new MemoryStream(input, 2 + skip, input.Length - 2 - skip)) // skip ZLIB bytes 
            using (var deflate = new DeflateStream(inputStream, CompressionMode.Decompress))
            using (var outputStream = new MemoryStream())
            {
                var buffer = new byte[1024];
                var read = deflate.Read(buffer, 0, buffer.Length);

                while (read == buffer.Length)
                {
                    outputStream.Write(buffer, 0, read);
                    read = deflate.Read(buffer, 0, buffer.Length);
                }

                outputStream.Write(buffer, 0, read);
                return outputStream.ToArray();
            }
        }
    }
}