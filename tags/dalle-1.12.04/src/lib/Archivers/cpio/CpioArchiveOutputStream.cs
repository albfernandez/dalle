/*

    Dalle - A split/join file utility library
	
	
	
    Copyright (C) 2010 Alberto Fernández  <infjaf@gmail.com>
    Original java code: commons-compress, from apache (http://commons.apache.org/compress/)
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
using System.Text;
using Dalle.Archivers;
using Dalle.Utilidades;

namespace Dalle.Archivers.cpio
{


	public class CpioArchiveOutputStream : ArchiveOutputStream
	{
		private CpioArchiveEntry entry;
		private bool closed = false;
		private bool finished;
		private short entryFormat;
		private long written;
		
		public CpioArchiveOutputStream (Stream outStream) : this(outStream, CpioConstants.FORMAT_NEW)
		{
			
		}
		
		public CpioArchiveOutputStream (Stream outStream, short format) : base(outStream)
		{
			switch (format) {
				case CpioConstants.FORMAT_NEW:
				case CpioConstants.FORMAT_NEW_CRC:
				case CpioConstants.FORMAT_OLD_ASCII:
				case CpioConstants.FORMAT_OLD_BINARY:
					this.entryFormat = format;
					break;
				default:
					throw new InvalidDataException ("Unknown format: " + format);
				
			}
			
		}


		public override void CloseArchiveEntry ()
		{
			// TODO
			if (finished) {
				throw new IOException ("Stream has already been finished");
			}
			
			EnsureOpen ();
			
			if (entry == null) {
				throw new IOException ("Trying to close non-existent entry");
			}
			
			if (this.entry.Size != this.written) {
				throw new IOException ("invalid entry size (expected "
	                    + this.entry.Size + " but got " + this.written
	                    + " bytes)");
			}
			Pad (this.entry.DataPadCount);
			/*
			 * TODO
		       if (this.entry.Format == CpioConstants.FORMAT_NEW_CRC) {
		           if (this.crc != this.entry.Chksum) {
		               throw new IOException("CRC Error");
		           }
		       }
	        */
	        this.entry = null;
	        //this.crc = 0;
	        this.written = 0;
		}

		public override ArchiveEntry CreateArchiveEntry (FileInfo inputFile, string entryName)
		{
			if(finished) {
	            throw new IOException("Stream has already been finished");
	        }
	        return new CpioArchiveEntry(inputFile, entryName);
		}

		public override void Finish ()
		{
			EnsureOpen();
	        if (finished) {
	            throw new IOException("This archive has already been finished");
	        }
	        
	        if (this.entry != null) {
	            throw new IOException("This archive contains unclosed entries.");
	        }
	        this.entry = new CpioArchiveEntry(this.entryFormat);
	        this.entry.Name=CpioConstants.CPIO_TRAILER;
	        this.entry.NumberOfLinks = 1;
	        WriteHeader(this.entry);
	        CloseArchiveEntry();
	        
	        finished = true;
		}

		public override void PutArchiveEntry (ArchiveEntry entry)
		{
			if (finished) {
				throw new IOException ("Stream has already been finished");
			}
			
	        CpioArchiveEntry e = (CpioArchiveEntry)entry;
			EnsureOpen ();
			if (this.entry != null) {
				CloseArchiveEntry ();
				// close previous entry
			}
			if (e.Time == -1) {
				e.Time = DateUtils.CurrentUnixTimeMillis;
			}
			
			short format = e.Format;
			if (format != this.entryFormat) {
				throw new IOException ("Header format: " + format + " does not match existing format: " + this.entryFormat);
			}
			// TODO Comprobar duplicados
						/*
		       if (this.names.put(e.getName(), e) != null) {
		           throw new IOException("duplicate entry: " + e.getName());
		       }
		*/
			WriteHeader (e);
			this.entry = e;
			this.written = 0;
		}
		private void WriteHeader (CpioArchiveEntry e)
		{
			switch (e.Format) {
			case CpioConstants.FORMAT_NEW:
				this.Write (UtArrays.ToAsciiBytes (CpioConstants.MAGIC_NEW));
				WriteNewEntry (e);
				break;
			case CpioConstants.FORMAT_NEW_CRC:
				this.Write (UtArrays.ToAsciiBytes (CpioConstants.MAGIC_NEW_CRC));
				WriteNewEntry (e);
				break;
			case CpioConstants.FORMAT_OLD_ASCII:
				this.Write (UtArrays.ToAsciiBytes (CpioConstants.MAGIC_OLD_ASCII));
				WriteOldAsciiEntry (e);
				break;
			case CpioConstants.FORMAT_OLD_BINARY:
				bool swapHalfWord = true;
				WriteBinaryLong (CpioConstants.MAGIC_OLD_BINARY, 2, swapHalfWord);
				WriteOldBinaryEntry (e, swapHalfWord);
				break;
			}
		}
		private void WriteNewEntry(CpioArchiveEntry entry) {
	        WriteAsciiLong(entry.Inode, 8, 16);
	        WriteAsciiLong(entry.Mode, 8, 16);
	        WriteAsciiLong(entry.UID, 8, 16);
	        WriteAsciiLong(entry.GID, 8, 16);
	        WriteAsciiLong(entry.NumberOfLinks, 8, 16);
	        WriteAsciiLong(entry.Time, 8, 16);
	        WriteAsciiLong(entry.Size, 8, 16);
	        WriteAsciiLong(entry.DeviceMaj, 8, 16);
	        WriteAsciiLong(entry.DeviceMin, 8, 16);
	        WriteAsciiLong(entry.RemoteDeviceMaj, 8, 16);
	        WriteAsciiLong(entry.RemoteDeviceMin, 8, 16);
	        WriteAsciiLong(entry.Name.Length + 1, 8, 16);
	        WriteAsciiLong(entry.Chksum, 8, 16);
	        WriteCString(entry.Name);
	        Pad(entry.HeaderPadCount);
	    }
	
	    private void WriteOldAsciiEntry( CpioArchiveEntry entry)
	    {
	        WriteAsciiLong(entry.Device, 6, 8);
	        WriteAsciiLong(entry.Inode, 6, 8);
	        WriteAsciiLong(entry.Mode, 6, 8);
	        WriteAsciiLong(entry.UID, 6, 8);
	        WriteAsciiLong(entry.GID, 6, 8);
	        WriteAsciiLong(entry.NumberOfLinks, 6, 8);
	        WriteAsciiLong(entry.RemoteDevice, 6, 8);
	        WriteAsciiLong(entry.Time, 11, 8);
	        WriteAsciiLong(entry.Name.Length + 1, 6, 8);
	        WriteAsciiLong(entry.Size, 11, 8);
	        WriteCString(entry.Name);
	    }
	
	    private void WriteOldBinaryEntry (CpioArchiveEntry entry, bool swapHalfWord) 
		{
	        WriteBinaryLong(entry.Device, 2, swapHalfWord);
	        WriteBinaryLong(entry.Inode, 2, swapHalfWord);
	        WriteBinaryLong(entry.Mode, 2, swapHalfWord);
	        WriteBinaryLong(entry.UID, 2, swapHalfWord);
	        WriteBinaryLong(entry.GID, 2, swapHalfWord);
	        WriteBinaryLong(entry.NumberOfLinks, 2, swapHalfWord);
	        WriteBinaryLong(entry.RemoteDevice, 2, swapHalfWord);
	        WriteBinaryLong(entry.Time, 4, swapHalfWord);
	        WriteBinaryLong(entry.Name.Length + 1, 2, swapHalfWord);
	        WriteBinaryLong(entry.Size, 4, swapHalfWord);
	        WriteCString(entry.Name);
	        Pad(entry.HeaderPadCount);
	    }
		
		
		private void EnsureOpen ()
		{
			if (this.closed) {
				throw new IOException ("Stream closed");
			}
		}
		public override void Close () 
		{
			 if(!finished) {
	            Finish();
	        }
	        
	        if (!this.closed) {
	            outStream.Close();
	            this.closed = true;
	        }
		}
		private void Pad(int count)
		{
	        if (count > 0){
	            byte[] buff = new byte[count];
	            this.Write(buff);
	        }
	    }
	
	    private void WriteBinaryLong(long number, int length,bool swapHalfWord) 
		{
	        byte[] tmp = CpioUtil.Long2byteArray(number, length, swapHalfWord);
	        this.Write(tmp);
	    }
	
	    private void WriteAsciiLong (long number, int length, int radix)
	    {
			// TODO
			/*
	    	StringBuilder tmp = new StringBuilder ();
	    	String tmpStr;
	    	// TODO
	    	if (radix == 16) {
	    		tmp.Append (Long.toHexString (number));
	    	} else if (radix == 8) {
	    		tmp.Append (Long.toOctalString (number));
	    	} else {
	    		tmp.Append (Long.toString (number));
	    	}
	    	
	        if (tmp.Length <= length) {
	    		long insertLength = length - tmp.Length;
	    		for (int pos = 0; pos < insertLength; pos++) {
	    			tmp.Insert (0, "0");
	    		}
	    		tmpStr = tmp.ToString ();
	    	} else {				
	            tmpStr = tmp.Substring(tmp.Length - length);
	        }
	        this.Write(ArchiveUtils.toAsciiBytes(tmpStr));*/
	    }
	
	    /**
	     * Writes an ASCII string to the stream followed by \0
	     * @param str the String to write
	     * @throws IOException if the string couldn't be written
	     */
	    private void WriteCString (String str)
	    {
	    	Write (UtArrays.ToAsciiBytes (str));
	    	WriteByte ((byte)'\0');
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