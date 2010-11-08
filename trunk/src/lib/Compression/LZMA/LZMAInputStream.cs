/*

    Dalle - A split/join file utility library
   Dalle.Compression.LZMA.LZMAInputStream
          Extract data from a LZMA compressed stream
          (No compression support implemented)
	
    Copyright (C) 2010 Alberto Fernández  <infjaf@gmail.com>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/
using System;
using System.IO;
using System.Threading;
using Dalle.Streams;
using SevenZip.Compression.LZMA;

namespace Dalle.Compression.LZMA
{


	public class LZMAInputStream : Stream
	{
		private Stream inStream; 
		private Thread writeThread;
		PipeStream pipe;
		private Decoder decoder = null;
		private long uncompressedSize = 0;
		private long compressedSize = 0;
		
		public LZMAInputStream (Stream inStream)
		{
			this.inStream = inStream;
			pipe = new PipeStream ();
			pipe.MaxBufferLength = PipeStream.MB * 4;
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
				return this.uncompressedSize;
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
			if (leidos == 0) {
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
