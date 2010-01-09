
using System;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;
namespace Dalle.Utilidades
{


	public class ChecksumStream : Stream
	{

		private IChecksum crc;
		private Stream stream;
		
		public IChecksum Crc {
			get { return crc;
			}}
		public ChecksumStream (Stream stream, IChecksum crc)
		{
			this.stream = stream;
			this.crc = crc;
		}
		public override void WriteByte (byte b)
		{
			if (crc != null) 
			{
				crc.Update ((int)b);
			}
			stream.WriteByte (b);
		}
		public override void Write (byte[] buffer, int offset, int count)
		{
			if (crc != null)
			{
				crc.Update (buffer, offset, count);
			}
			stream.Write (buffer, offset, count);
		}
		public override bool CanRead {
			get {
				return stream.CanRead;
			}
		}
		public override bool CanSeek {
			get { return stream.CanSeek; }
		}
		public override bool CanWrite {
			get {
				return stream.CanWrite;
			}
		}
		public override long Length {
			get 
			{
				return stream.Length;
			}
		}
		public override long Position {
			get {
				return stream.Position;
			}
			set {
				stream.Position = value;
			}
		}
		public override void Flush ()
		{
			stream.Flush ();
		}
		public override int Read (byte[] buffer, int offset, int count)
		{
			return stream.Read (buffer, offset, count);
		}
		public override long Seek (long offset, SeekOrigin origin)
		{
			return stream.Seek (offset, origin);
		}
		public override void SetLength (long length)
		{
			stream.SetLength (length);
		}
		public override void Close () {
			stream.Close();
		}






		

	}
}
