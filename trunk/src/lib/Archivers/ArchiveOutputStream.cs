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
using Dalle.Streams;


namespace Dalle.Archivers
{


	public abstract class ArchiveOutputStream : Stream
	{
		private byte[] oneByte = new byte[1];
    	public const int BYTE_MASK = 0xFF;
		
		private int bytesWrited = 0;
		protected Stream outStream;
		
		public abstract void PutArchiveEntry(ArchiveEntry entry);
		public abstract void CloseArchiveEntry();
		public abstract void Finish();
		public abstract ArchiveEntry CreateArchiveEntry(FileInfo inputFile, String entryName);
			
		public ArchiveOutputStream (Stream outStream)
		{
			this.outStream = outStream;
		}

		public override bool CanRead {
			get {
				return false;
			}
		}

		public override bool CanSeek {
			get {
				return false;
			}
		}

		public override bool CanWrite {
			get {
				return true;
			}
		}

		public override long Length {
			get {
				throw new System.NotImplementedException();
			}
		}

		public override long Position {
			get {
				return bytesWrited;
			}
			set {
				throw new System.NotImplementedException();
			}
		}

		public override void Close ()
		{
			this.outStream.Close ();
		}


		public override void Flush ()
		{
			this.outStream.Flush ();
		}

		public override int Read (byte[] buffer, int offset, int count)
		{
			throw new System.NotImplementedException();
		}

		public override int ReadByte ()
		{
			throw new System.NotImplementedException ();
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
			this.outStream.Write (buffer, offset, count);
			this.Count (count);
		}

		public override void WriteByte (byte b)
		{
			oneByte[0] = (byte)(b & BYTE_MASK);
			this.Write (oneByte, 0, 1);
		}
		public void Count (int writed)
		{
			if (writed > 0) {
				bytesWrited = bytesWrited + writed;
			}
		}
		public void Write (byte[] buffer)
		{
			Write (buffer, 0, buffer.Length);
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