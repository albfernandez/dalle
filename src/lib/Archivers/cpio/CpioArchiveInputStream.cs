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
using Dalle.Utilidades;
using Dalle.Streams;
/**
 * CPIOArchiveInputStream is a stream for reading cpio streams. All formats of
 * cpio are supported (old ascii, old binary, new portable format and the new
 * portable format with crc).
 * <p/>
 * <p/>
 * The stream can be read by extracting a cpio entry (containing all
 * informations about a entry) and afterwards reading from the stream the file
 * specified by the entry.
 * <p/>
 * <code><pre>
 * CPIOArchiveInputStream cpioIn = new CPIOArchiveInputStream(
 *         new FileInputStream(new File(&quot;test.cpio&quot;)));
 * CPIOArchiveEntry cpioEntry;
 * <p/>
 * while ((cpioEntry = cpioIn.getNextEntry()) != null) {
 *     System.out.println(cpioEntry.getName());
 *     int tmp;
 *     StringBuffer buf = new StringBuffer();
 *     while ((tmp = cpIn.read()) != -1) {
 *         buf.append((char) tmp);
 *     }
 *     System.out.println(buf.toString());
 * }
 * cpioIn.close();
 * </pre></code>
 * <p/>
 * Note: This implementation should be compatible to cpio 2.5
 * 
 * This class uses mutable fields and is not considered to be threadsafe.
 * 
 * Based on code from the jRPM project (jrpm.sourceforge.net)
 */

//    case 'c':	/* Use the old portable ASCII format.  */
//     case 'H':		/* Header format name.  */
// crc newc odc bin ustar tar (all-caps also recognized)"), arg);


namespace Dalle.Archivers.cpio
{


	public class CpioArchiveInputStream : ArchiveInputStream
	{


		private CpioArchiveEntry currentEntry;
		private long entryBytesRead = 0;
		private byte[] tmpbuf = new byte[4096];

		
		public CpioArchiveInputStream (Stream inputStream) : base(inputStream)
		{
		}
		public override void Close ()
		{
			if (!this.closed) {
				this.inputStream.Close ();
				this.closed = true;
			}
		}

		public override ArchiveEntry GetNextEntry ()
		{
			return GetNextCPIOEntry ();
		}
		private void CloseEntry ()
		{
			EnsureOpen ();
			
	        while (this.Read (this.tmpbuf, 0, this.tmpbuf.Length) > 0) {
				// do nothing
			}
	    }
		private void EnsureOpen ()
		{
			if (this.closed) 
			{
				throw new IOException ("Stream closed");
			}
		}
		
			/**
	     * Reads the next CPIO file entry and positions stream at the beginning of
	     * the entry data.
	     * 
	     * @return the CPIOArchiveEntry just read
	     * @throws IOException
	     *             if an I/O error has occurred or if a CPIO file error has
	     *             occurred
	     */
	    public CpioArchiveEntry GetNextCPIOEntry ()
	    {
	    	EnsureOpen ();
	    	if (this.currentEntry != null) {
	    		CloseEntry ();
	    	}
	    	byte[] magic = new byte[2];
	    	ReadFully (magic, 0, magic.Length);
	    	if (CpioUtil.ByteArray2long (magic, false) == CpioConstants.MAGIC_OLD_BINARY) {
	    		this.currentEntry = ReadOldBinaryEntry (false);
	    	} else if (CpioUtil.ByteArray2long (magic, true) == CpioConstants.MAGIC_OLD_BINARY) {
	    		this.currentEntry = ReadOldBinaryEntry (true);
	    	} else {
	    		byte[] more_magic = new byte[4];
	    		ReadFully (more_magic, 0, more_magic.Length);
	    		byte[] tmp = new byte[6];
	    		System.Array.Copy (magic, tmp, magic.Length);
	    		System.Array.Copy (more_magic, 0, tmp, magic.Length, more_magic.Length);
	    		//System.arraycopy(magic, 0, tmp, 0, magic.length);
	    		//System.arraycopy(more_magic, 0, tmp, magic.length,more_magic.length);
	    		String magicString = UtArrays.ToAsciiString (tmp);
	    		if (magicString.Equals (CpioConstants.MAGIC_NEW)) {
	    			this.currentEntry = ReadNewEntry (false);
	    		} else if (magicString.Equals (CpioConstants.MAGIC_NEW_CRC)) {
	    			this.currentEntry = ReadNewEntry (true);
	    		} else if (magicString.Equals (CpioConstants.MAGIC_OLD_ASCII)) {
	    			this.currentEntry = ReadOldAsciiEntry ();
	    		} else {
	    			string tmpString = "";
	    			foreach (byte b in tmp) {
	    				tmpString += b.ToString ("X") + ",";
	    			}
	    			throw new IOException ("Unknown magic [" + magicString + "][" + tmpString + "]. Occured at byte: " + Position + "/0x" + Position.ToString ("X"));
	    		}
	    	}
	    	
	    	this.entryBytesRead = 0;
	    
	    

	    	
			if (this.currentEntry.Name.Equals (CpioConstants.CPIO_TRAILER)) {
				int contador = 0;
				while (this.inputStream.ReadByte() >= 0){
					contador++;
				}
				//Console.WriteLine("Bytes al final = " + contador);
	    		//Console.WriteLine ("TRAILER---" + this.currentEntry.Size);
				//Console.WriteLine("Trailer--" + this.currentEntry.HeaderPadCount + "/" + this.currentEntry.DataPadCount);
	    		return null;
	    	}
	    	
	    	this.dataStream = new SizeLimiterStream (this.inputStream, this.currentEntry.Size);
	    	if (this.currentEntry.Format == CpioConstants.FORMAT_NEW_CRC) {
	    		this.dataStream = new ChecksumStream (dataStream, new CpioChecksum ());
			}
			
	        return this.currentEntry;
	    }
		private CpioArchiveEntry ReadNewEntry (bool hasCrc)
		{
			CpioArchiveEntry ret;
			if (hasCrc) {
				ret = new CpioArchiveEntry (CpioConstants.FORMAT_NEW_CRC);
			} else {
				ret = new CpioArchiveEntry (CpioConstants.FORMAT_NEW);
			}
	
	        ret.Inode = ReadAsciiLong (8, 16);
			long mode = ReadAsciiLong (8, 16);
			if (mode != 0) {
				// mode is initialised to 0
				ret.Mode = mode;
	        }
	        ret.UID = ReadAsciiLong(8, 16);
	        ret.GID = ReadAsciiLong(8, 16);
	        ret.NumberOfLinks = ReadAsciiLong(8, 16);
	        ret.Time = ReadAsciiLong(8, 16);
	        ret.Size = ReadAsciiLong(8, 16);
	        ret.DeviceMaj = ReadAsciiLong(8, 16);
	        ret.DeviceMin = ReadAsciiLong(8, 16);
	        ret.RemoteDeviceMaj  = ReadAsciiLong(8, 16);
	        ret.RemoteDeviceMin = ReadAsciiLong(8, 16);
	        long namesize = ReadAsciiLong(8, 16);
	        ret.Chksum= ReadAsciiLong(8, 16);
	        String name = ReadCString((int) namesize);
	        ret.Name = name;
	        if (mode == 0 && !name.Equals(CpioConstants.CPIO_TRAILER)){
	            throw new IOException("Mode 0 only allowed in the trailer. Found entry name: "+name + " Occured at byte: " + Position);
	        }
	        Skip(ret.HeaderPadCount);
	
	        return ret;
	    }
		private CpioArchiveEntry ReadOldAsciiEntry()  {
	        CpioArchiveEntry ret = new CpioArchiveEntry(CpioConstants.FORMAT_OLD_ASCII);
	
	        ret.Device = ReadAsciiLong(6, 8);
	        ret.Inode = ReadAsciiLong(6, 8);
	        long mode = ReadAsciiLong(6, 8);
	        if (mode != 0) {
	            ret.Mode=mode;
	        }
	        ret.UID =ReadAsciiLong(6, 8);
	        ret.GID=ReadAsciiLong(6, 8);
	        ret.NumberOfLinks=ReadAsciiLong(6, 8);
	        ret.RemoteDevice=ReadAsciiLong(6, 8);
	        ret.Time=ReadAsciiLong(11, 8);
	        long namesize = ReadAsciiLong(6, 8);
	        ret.Size=ReadAsciiLong(11, 8);
	        String name = ReadCString((int) namesize);
	        ret.Name=name;
	        if (mode == 0 && !name.Equals(CpioConstants.CPIO_TRAILER)){
	            throw new IOException("Mode 0 only allowed in the trailer. Found entry: "+ name + " Occured at byte: " + Position);
	        }
	
	        return ret;
	    }
		
	    private CpioArchiveEntry ReadOldBinaryEntry (bool swapHalfWord)
	    {
	    	CpioArchiveEntry ret = new CpioArchiveEntry (CpioConstants.FORMAT_OLD_BINARY);
	
	        ret.Device = ReadBinaryLong (2, swapHalfWord);
	    	ret.Inode = ReadBinaryLong (2, swapHalfWord);
	    	long mode = ReadBinaryLong (2, swapHalfWord);
	    	if (mode != 0) {
	    		ret.Mode = mode;
	    	}
	    	ret.UID = ReadBinaryLong (2, swapHalfWord);
	    	ret.GID = ReadBinaryLong (2, swapHalfWord);
	    	ret.NumberOfLinks = ReadBinaryLong (2, swapHalfWord);
	    	ret.RemoteDevice = ReadBinaryLong (2, swapHalfWord);
	    	ret.Time = ReadBinaryLong (4, swapHalfWord);
	    	long namesize = ReadBinaryLong (2, swapHalfWord);
	    	ret.Size = ReadBinaryLong (4, swapHalfWord);
	    	String name = ReadCString ((int)namesize);
	    	ret.Name = name;
	    	if (mode == 0 && !name.Equals(CpioConstants.CPIO_TRAILER)){
	            throw new IOException("Mode 0 only allowed in the trailer. Found entry: "+name + "Occured at byte: " + Position);
	        }
	        Skip(ret.HeaderPadCount);
	
	        return ret;
	    }
	
	    private String ReadCString (int length)
	    {
	    	byte[] tmpBuffer = new byte[length];
	    	ReadFully (tmpBuffer, 0, tmpBuffer.Length);
	    	return UtArrays.LeerTexto (tmpBuffer, 0, tmpBuffer.Length - 1);
	    }
		


	    /**
	     * Checks if the signature matches one of the following magic values:
	     * 
	     * Strings:
	     *  
	     * "070701" - MAGIC_NEW
	     * "070702" - MAGIC_NEW_CRC
	     * "070707" - MAGIC_OLD_ASCII
	     * 
	     * Octal Binary value:
	     * 
	     * 070707 - MAGIC_OLD_BINARY (held as a short) = 0x71C7 or 0xC771
	     */
	    public static bool Matches(byte[] signature, int length) {
	        if (length < 6) {
	            return false;
	        }
	        
	        // Check binary values
	        if (signature[0] == 0x71 && (signature[1] & 0xFF) == 0xc7) {
	            return true;
	        }
	        if (signature[1] == 0x71 && (signature[0] & 0xFF) == 0xc7) {
	            return true;
	        }
	
	        // Check Ascii (String) values
	        // 3037 3037 30nn
	        if (signature[0] != 0x30) {
	            return false;
	        }
	        if (signature[1] != 0x37) {
	            return false;
	        }
	        if (signature[2] != 0x30) {
	            return false;
	        }
	        if (signature[3] != 0x37) {
	            return false;
	        }
	        if (signature[4] != 0x30) {
	            return false;
	        }
	        // Check last byte
	        if (signature[5] == 0x31) {
	            return true;
	        }
	        if (signature[5] == 0x32) {
	            return true;
	        }
	        if (signature[5] == 0x37) {
	            return true;
	        }
	
	        return false;
	    }

		private void Skip (int bytes)
		{
			//Console.WriteLine ("Skip:" + bytes);
			byte[] buff = new byte[4];
			// Cannot be more than 3 bytes
			if (bytes > 0) {
				ReadFully (buff, 0, bytes);
			}
		}
		public override int Read (byte[] b, int off, int len)
		{
			int tmp = base.Read (b, off, len);
			this.entryBytesRead += tmp;
			if (tmp != 0 && this.entryBytesRead == this.currentEntry.Size)
			{
				// Comprobar crc
				if (this.currentEntry.Format == CpioConstants.FORMAT_NEW_CRC) 
				{
					if (this.currentEntry.Chksum != ((ChecksumStream)dataStream).Crc.Value) {
						throw new IOException ("CRC Error. Occured at byte: " + Position);
					}
				}				
				this.Skip (this.currentEntry.DataPadCount);

			}
			return tmp;
		}
	
	
	    private int ReadFully (byte[] b, int off, int len)
	    {
	    	if (len < 0) {
	    		throw new IndexOutOfRangeException ();
	        }
	        int n = 0;
	        while (n < len) {
	            int count = this.inputStream.Read(b, off + n, len - n);
	            this.Count(count);
	            if (count < 0) {
	                throw new EndOfStreamException();
	            }
	            n += count;
	        }
	        return n;
	    }
	
	    private long ReadBinaryLong (int length, bool swapHalfWord)
	    {
	    	byte[] tmp = new byte[length];
	        ReadFully(tmp, 0, tmp.Length);
	        return CpioUtil.ByteArray2long(tmp, swapHalfWord);
	    }
	
	    private long ReadAsciiLong (int length, int radix)
	    {
	    	byte[] tmpBuffer = new byte[length];
	    	ReadFully (tmpBuffer, 0, tmpBuffer.Length);
	    	//return Long.parseLong(UtArrays.ToAsciiString(tmpBuffer), radix);
	    	return Convert.ToInt64 (UtArrays.ToAsciiString (tmpBuffer), radix);
	    }
		
		protected override void Count (int c)
		{
			//Console.WriteLine ("Count " + c);
			base.Count(c);
			/*if (this.Position != inputStream.Position){
				Console.WriteLine(" --> " + this.Position + "/" + inputStream.Position);
			}*/
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