/*

    Dalle - A split/join file utility library
	
    Copyright (C) 2003-2010  Alberto Fernández <infjaf@gmail.com>

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

namespace Dalle.Streams
{


	public class SizeLimiterStream : Stream
	{

		private Stream stream;
		private long maxSize;
		private long alreadyRead = 0;
		private long length;

		public SizeLimiterStream (Stream stream, long maxSize)
		{
			this.stream = stream;
			this.maxSize = maxSize;
			try {
				this.length = stream.Length - stream.Position;
			}
			catch (Exception) 
			{
				this.length = maxSize;
			}
			if (maxSize > 0) {
				this.length = Math.Min (this.length, maxSize);
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
				return this.length;
			}
		}
		public override long Position {
			get {
				return alreadyRead;
			}
			set {
				throw new System.NotImplementedException ();
			}
		}
		public override void Flush ()
		{
			throw new System.NotImplementedException ();
		}
		public override int Read (byte[] buffer, int offset, int count)
		{
			if (maxSize >= 0) {
				int leidos = stream.Read (buffer, offset, (int)Math.Min (count, maxSize - alreadyRead));
				alreadyRead += leidos;
				return leidos;
			}
			else {
				return stream.Read (buffer, offset, count);
			}
		}
		
		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new System.NotImplementedException ();
		}
		public override void SetLength (long value)
		{
			throw new System.NotImplementedException ();
		}

		
		public override void Write (byte[] buffer, int offset, int count)
		{
			throw new System.NotImplementedException ();
		}

	}
}
