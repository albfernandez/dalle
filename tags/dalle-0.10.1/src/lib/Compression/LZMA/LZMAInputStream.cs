
using System;
using System.IO;
using System.Threading;
using PipeStream;
using SevenZip.Compression.LZMA;

namespace Dalle.Compression.LZMA
{


	public class LZMAInputStream : Stream
	{
		private Stream inStream; 
		private Thread writeThread;
		PipeStream.PipeStream pipe;
		private Decoder decoder = null;
		private long uncompressedSize = 0;
		private long compressedSize = 0;
		
		public LZMAInputStream (Stream inStream)
		{
			this.inStream = inStream;
			pipe = new PipeStream.PipeStream ();
			pipe.MaxBufferLength = PipeStream.PipeStream.MB * 4;
			pipe.BlockLastReadBuffer = true;
			
			this.InitDecoder ();
			
			writeThread = new Thread (new ThreadStart (Writer));
			writeThread.Start ();
		
		}
		private void InitDecoder ()
		{
			decoder = new SevenZip.Compression.LZMA.Decoder ();
			byte[] properties = new byte[5];
			inStream.Read (properties, 0, 5);
			decoder.SetDecoderProperties (properties);
			for (int i = 0; i < 8; i++)
			{
				int v = inStream.ReadByte ();
				uncompressedSize |= ((long)(byte)v) << (8 * i);
			}
			compressedSize = inStream.Length - inStream.Position;
		}
		private void Writer ()
		{
			
			decoder.Code (inStream, pipe, compressedSize, uncompressedSize, null);			
			pipe.BlockLastReadBuffer = false;
			pipe.Flush ();
		}
		
		
		public long UncompressedSize {
			get {
				return uncompressedSize;
			}
		}

		public override bool CanRead {
			get {
				return true;
			}
		}

		public override bool CanSeek {
			get {
				return false;
			}
		}

		public override bool CanWrite {
			get {
				return false;
			}
		}

		public override long Length {
			get {
				throw new System.NotImplementedException();
			}
		}

		public override long Position {
			get {
				throw new System.NotImplementedException();
			}
			set {
				throw new System.NotImplementedException();
			}
		}

		public override void Flush ()
		{
			throw new System.NotImplementedException();
		}

		public override int Read (byte[] buffer, int offset, int count)
		{
			return pipe.Read (buffer, offset, count);
		}

		public override int ReadByte ()
		{
			byte[] one = new byte[1];
			int leidos = Read (one, 0, 1);
			if (leidos == 1) {
				return -1;
			}
			else {
				return one[0];
			}
		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new System.NotImplementedException();
		}

		public override void SetLength (long value)
		{
			throw new System.NotImplementedException();
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			throw new System.NotImplementedException();
		}

		public override void WriteByte (byte value)
		{
			base.WriteByte(value);
		}

		public override void Close ()
		{
			inStream.Close();
		}
	}
}
