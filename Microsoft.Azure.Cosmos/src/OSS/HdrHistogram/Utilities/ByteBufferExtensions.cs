// ------------------------------------------------------------
// The code in this repository code was written by Lee Campbell, as a
// derived work from the original Java by Gil Tene of Azul Systems and
// Michael Barker, and released to the public domain, as explained
// at http://creativecommons.org/publicdomain/zero/1.0/
// ------------------------------------------------------------

// This file isn't generated, but this comment is necessary to exclude it from StyleCop analysis.
// <auto-generated/>

using System.IO;
using System.IO.Compression;
using Microsoft.Azure.Cosmos;
using Microsoft.IO;

namespace HdrHistogram.Utilities
{
    internal static class ByteBufferExtensions
    {
        /// <summary>
        /// Copies compressed contents from <paramref name="source"/> into the <paramref name="target"/> from the <paramref name="targetOffset"/>
        /// </summary>
        /// <param name="target">The <see cref="ByteBuffer"/> that will be written to.</param>
        /// <param name="source">The source <see cref="ByteBuffer"/> to read the data from.</param>
        /// <param name="targetOffset">The <paramref name="target"/> buffer's offset to start writing from.</param>
        /// <returns>The number of bytes written.</returns>
        public static int CompressedCopy(this ByteBuffer target, ByteBuffer source, int targetOffset)
        {
            byte[] compressed;
            using (var ms = new MemoryStream(source.ToArray()))
            {
                compressed = Compress(ms);
            }
            target.BlockCopy(compressed, 0, targetOffset, compressed.Length);
            return compressed.Length;
        }

        private static byte[] Compress(Stream input)
        {
            using (var compressStream = StreamManager.GetStream(nameof(Compress)) as RecyclableMemoryStream)
            {
                //Add the RFC 1950 headers.
                compressStream.WriteByte(0x58);
                compressStream.WriteByte(0x85);
                using (var compressor = new DeflateStream(compressStream, CompressionMode.Compress, leaveOpen: true))
                {
                    input.CopyTo(compressor);
                }
                return compressStream.GetBuffer();
            }
        }
    }
}