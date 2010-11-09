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
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Dalle.Streams
{
	public class PipeStream : Stream
	{		
		public  const long DEFAULT_BUFFER_LENGTH = 2 * 1024 * 1024;
		
		private readonly Queue<byte> ibuffer = new Queue<byte> ();
		private bool writeFinished = false;		
		private long bufferLength = DEFAULT_BUFFER_LENGTH;

		public override bool CanRead {
			get { return true; }
		}
		public override bool CanSeek {
			get { return false; }
		}
		public override bool CanWrite {
			get { return true; }
		}
		public override long Length {
			get { return ibuffer.Count; }
		}
		public override long Position {
			get { return 0; }
			set {
				throw new NotImplementedException ();
			}
		}

		public PipeStream ()
		{
		}
		public PipeStream (long bufferLength)
		{
			this.bufferLength = bufferLength;
		}

		public new void Dispose ()
		{
			ibuffer.Clear ();
		}

		public override void Flush ()
		{
			writeFinished = true;
			lock (ibuffer) {
				Monitor.Pulse (ibuffer);
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
		public override int Read (byte[] buffer, int offset, int count)
		{
			if (buffer == null)
				throw new ArgumentException ("buffer is null");
			if (offset + count > buffer.Length)
				throw new ArgumentException ("The sum of offset and count is greater than the buffer length.");
			if (offset < 0)
				throw new ArgumentOutOfRangeException ("offset", "offset must be positive");
			if (count < 0)
				throw new ArgumentOutOfRangeException ("count", "count must be positive");
			
			if (count == 0) {
				return 0;
			}
			
			int readLength = 0;
			
			lock (ibuffer) {
				while (readLength < count) {
					if (Length == 0 && !writeFinished) {
						Monitor.Pulse (ibuffer);
						Monitor.Wait (ibuffer);
					}
					if (Length == 0) 
					{
						break;
					}
					buffer[offset+readLength] = ibuffer.Dequeue ();
					readLength++;
				}				
				Monitor.Pulse (ibuffer);
			}
			return readLength;
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			if (buffer == null)
				throw new ArgumentException ("buffer is null");
			if (offset + count > buffer.Length)
				throw new ArgumentException ("The sum of offset and count is greater than the buffer length. ");
			if (offset < 0)
				throw new ArgumentOutOfRangeException ("offset", "offset must be positive");
			if (count < 0)
				throw new ArgumentOutOfRangeException ("count", "count must be positive");
			if (count == 0)
				return;
			
			lock (ibuffer) {
				for (int i = offset; i < offset + count; i++) {
					if (Length >= bufferLength) {
						Monitor.Pulse (ibuffer);
						Monitor.Wait (ibuffer);
					}
					ibuffer.Enqueue (buffer[i]);					
				}				
				Monitor.Pulse (ibuffer);
			}
		}		
	}
}
