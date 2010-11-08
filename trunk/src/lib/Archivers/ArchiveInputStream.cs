/*

    Dalle - A split/join file utility library
	
	
	
    Copyright (C) 2010 Alberto Fernández  <infjaf@gmail.com>
    Original java code: commons-commpress, from apache (http://commons.apache.org/compress/)
    C# translation by - Alberto Fernández  <infjaf@gmail.com>

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

namespace Dalle.Archivers
{	
	public abstract class ArchiveInputStream : Stream
	{
		private byte[] SINGLE = new byte[1];
		private static readonly int BYTE_MASK = 0xFF;
		private int bytesRead = 0;
		protected Stream inputStream;
		protected Stream dataStream;
		protected bool closed;
		public ArchiveInputStream (Stream inputStream)
		{
			this.inputStream = inputStream;
		}
		
		public abstract ArchiveEntry GetNextEntry ();		

		public override int ReadByte ()
		{
			int num = Read (SINGLE, 0, 1);
			return num <= 0 ? -1 : SINGLE[0] & BYTE_MASK;
		}
		protected virtual void Count (int read)
		{
			if (read != -1) 
			{
				bytesRead = bytesRead + read;
			}
		}
		
		public override bool CanRead {
			get { return true; }
		}
		public override bool CanWrite {
			get { return false; }
		}

		public override long Position {
			get { return bytesRead; }
			set {
				throw new NotImplementedException ();
			}
		}

		public override bool CanSeek {
			get {
				throw new System.NotImplementedException();
			}
		}

		public override long Length {
			get {
				throw new System.NotImplementedException();
			}
		}
		public override void Flush ()
		{
			inputStream.Flush ();
		}
		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new NotImplementedException ();
		}
		public override void SetLength (long value)
		{
			throw new NotImplementedException ();
		}

		public override int Read (byte[] buffer, int offset, int count)
		{
			if (dataStream != null)
			{
				int readed = dataStream.Read (buffer, offset, count);
				this.Count (readed);
				return readed;
			}
			return -1;
		
		}
		public int Read (byte[] buffer)
		{
			return Read (buffer, 0, buffer.Length);
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			throw new System.NotImplementedException();
		}

		public override void WriteByte (byte value)
		{
			throw new System.NotImplementedException();
		}
		public long TotalLength {
			get { return -1; }
		}
		public long EntriesCount {
			get { return -1; }
		}
		public long CurrentEntryCount {
			get { return -1; }
		}
		
		
	}
}


/* The original Java file had this header:
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */