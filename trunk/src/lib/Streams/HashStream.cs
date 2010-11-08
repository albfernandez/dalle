/*

    Dalle - A split/join file utility library
	
	
	
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
using System.Security.Cryptography;

namespace Dalle.Streams
{


	public class HashStream : Stream
	{

		private Stream baseStream;
		private HashAlgorithm h;
		private string expected = null;
		
		private byte[] SINGLE = new byte[1];
		private static readonly int BYTE_MASK = 0xFF;
		
		public override bool CanRead {
			get {
				return baseStream.CanRead;
			}
		}
		
		public override bool CanSeek {
			get {
				return baseStream.CanSeek;
			}
		}
		
		public override bool CanTimeout {
			get {
				return baseStream.CanTimeout;
			}
		}
		
		public override bool CanWrite {
			get {
				return baseStream.CanWrite;
			}
		}
		
		public override long Length {
			get {
				return baseStream.Length;
			}
		}
		
		public override long Position {
			get {
				return baseStream.Position;
			}
			set {
				baseStream.Position = value;
			}
		}
		
		
		
		public string Hash {
			get { return this.hash;}
		}
		
		private string hash;
		
		
		public HashStream (Stream baseStream, HashAlgorithm h)
		{
			this.baseStream = baseStream;
			this.h = h;
		}
		public HashStream (Stream baseStream, HashAlgorithm h, string expected)
		{
			this.baseStream = baseStream;
			this.h = h;
			this.expected = expected;

		}
		public override int Read (byte[] buffer, int offset, int count)
		{
			if (baseStream.CanRead) {
				int bytesReaded = baseStream.Read (buffer, offset, count);
				h.TransformBlock (buffer, offset, bytesReaded, null, 0);
				return bytesReaded;
			}
			throw new IOException ("Cant read from stream");
			
		}
		public override void Close ()
		{
			baseStream.Close ();
			h.TransformFinalBlock (new byte[0], 0, 0);
			byte[] res = h.Hash;
			this.hash = Dalle.Utilidades.UtilidadesCadenas.FormatHexHash (res).ToLower ();
			if (expected != null) {				
				if (!expected.Equals (this.hash))
				{
					throw new IOException ("Verification failed");
				}
			}
			
		}
		
		public override void Flush ()
		{
			baseStream.Flush ();
		}
		
		public override int ReadByte ()
		{
			int num = Read (SINGLE, 0, 1);
			return num <= 0 ? -1 : SINGLE[0] & BYTE_MASK;
		}
		
		public override long Seek (long offset, SeekOrigin origin)
		{
			return baseStream.Seek (offset, origin);
		}
		
		public override void SetLength (long val)
		{
			baseStream.SetLength (val);
		}
		
		public override void Write (byte[] buffer, int offset, int count)
		{
			if (baseStream.CanWrite) 
			{
				h.TransformBlock (buffer, offset, count, null, 0);
				baseStream.Write (buffer, offset, count);
				return;
			
			}
			throw new IOException ("Can not write to stream");
		}
		
		public override void WriteByte (byte b)
		{
			SINGLE[0] = (byte)(b & BYTE_MASK);
			this.Write (SINGLE, 0, 1);
		}
		
		
		
		
		
		
		
		
	}
}
