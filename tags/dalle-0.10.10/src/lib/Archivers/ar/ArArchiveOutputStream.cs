/*

    Dalle - A split/join file utility library
	
	
	
    Copyright (C) 2010
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
using Dalle.Archivers;
using Dalle.Streams;

namespace Dalle.Archivers.ar
{


	public class ArArchiveOutputStream : ArchiveOutputStream
	{
		private ArArchiveEntry currentEntry;
		private bool haveUnclosedEntry = false;
		private bool finished = false;
		private long entryOffset;
		
		public ArArchiveOutputStream (Stream outStream) : base(outStream)
		{
		}
		private long WriteArchiveHeader ()
		{
			byte[] header = new System.Text.ASCIIEncoding ().GetBytes (ArArchiveEntry.HEADER);
			this.outStream.Write (header, 0, header.Length);
			this.Count (header.Length);
			return header.Length;
		}

		public override void CloseArchiveEntry ()
		{
			if (finished) {
				throw new IOException ("Stream has already been finished");
			}
			if (currentEntry == null || !haveUnclosedEntry) {
				throw new IOException ("No current entry to close");
			}
			if ((this.Position % 2) != 0) {
				this.outStream.WriteByte ((byte)'\n');
				// Pad byte
				this.Count (1);
	        }
	        haveUnclosedEntry = false;
		}

		public override ArchiveEntry CreateArchiveEntry (FileInfo inputFile, string entryName)
		{
			if (finished) {
				throw new IOException ("Stream has already been finished");
			}
			return new ArArchiveEntry (inputFile, entryName);
		}

		public override void Finish ()
		{
			if (haveUnclosedEntry) {
				throw new IOException ("This archive contains unclosed entries.");
			} else if (finished) {
				throw new IOException ("This archive has already been finished");
			}
			finished = true;
		}

		public override void PutArchiveEntry (ArchiveEntry pEntry)
		{
			if (finished) {
				throw new IOException ("Stream has already been finished");
			}
			
			ArArchiveEntry pArEntry = (ArArchiveEntry)pEntry;
			if (currentEntry == null) {
				// First entry
				WriteArchiveHeader ();
			} else {
				if (currentEntry.Size != entryOffset) {
					throw new IOException ("length does not match entry (" + currentEntry.Size + " != " + entryOffset);
				}
				
				if (haveUnclosedEntry) {
					CloseArchiveEntry ();
				}
			}
			
			currentEntry = pArEntry;
			
			WriteEntryHeader (pArEntry);
			
			entryOffset = 0;
			haveUnclosedEntry = true;
		}
		public override void Write (byte[] buffer, int offset, int count)
		{
			base.Write (buffer, offset, count);
			if (count > 0) 
			{
				entryOffset += count;
			}
		}
		private long Write (String n)
		{
			byte[] bytes = new System.Text.ASCIIEncoding ().GetBytes (n);
			this.Write(bytes, 0, bytes.Length);
        	return bytes.Length;
		}
		private long Fill (long pOffset, long pNewOffset, char pFill) 
		{ 
		         long diff = pNewOffset - pOffset;
		
		        if (diff > 0) {
		            for (int i = 0; i < diff; i++) {
		                WriteByte((byte)pFill);
		            }
		        }
		
		        return pNewOffset;
		    }

		private long WriteEntryHeader (ArArchiveEntry pEntry)
		{

	        long offset = 0;
	
			string longName = null;
			
	        string n = pEntry.Name;
			if (n.Length > 16) {
				// BSD variant
				// Seems to be incorrect or not work on debian ar
				//longName = n;
				//n = "#1/" + n.Length;
				
				throw new IOException ("filename too long, > 16 chars: " + n);
			}
			offset += Write (n);
			
			offset = Fill (offset, 16, ' ');
			// TODO: Ojo tiempo .net
			//String m = "" + (pEntry.LastModified / 1000);
			String m = "1264026598";
			if (m.Length > 12) {
				throw new IOException ("modified too long");
			}
			offset += Write (m);
			
			offset = Fill (offset, 28, ' ');
			String u = "" + pEntry.UserId;
			if (u.Length > 6) {
				throw new IOException ("userid too long");
			}
			offset += Write (u);
			
			offset = Fill (offset, 34, ' ');
			String g = "" + pEntry.GroupId;
			if (g.Length > 6) {
				throw new IOException ("groupid too long");
			}
			offset += Write (g);
			
			offset = Fill (offset, 40, ' ');
			//String fm = "" + Integer.toString(pEntry.Mode, 8);
			String fm = "0100644";
			if (fm.Length > 8) {
				throw new IOException ("filemode too long");
			}
			offset += Write (fm);
			
			offset = Fill (offset, 48, ' ');
			String s = "" + pEntry.Size;
			if (s.Length > 10) {
				throw new IOException ("size too long");
			}
			offset += Write (s);
			
			offset = Fill (offset, 58, ' ');
			
			offset += Write (ArArchiveEntry.TRAILER);
			if (longName != null)
			{
				offset += Write (longName);
			}
	        return offset;
		}
		public override void Close () 
		{
			if(!finished) {
	            Finish();
	        }
	        this.outStream.Close();
	        currentEntry = null;
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