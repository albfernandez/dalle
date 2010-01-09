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
