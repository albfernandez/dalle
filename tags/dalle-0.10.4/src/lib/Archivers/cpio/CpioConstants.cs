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

namespace Dalle.Archivers.cpio
{


	public class CpioConstants
	{
		/** magic number of a cpio entry in the new format */
	    public const String MAGIC_NEW = "070701";
	
	    /** magic number of a cpio entry in the new format with crc */
	    public const String MAGIC_NEW_CRC = "070702";
	
	    /** magic number of a cpio entry in the old ascii format */
	    public const String MAGIC_OLD_ASCII = "070707";
	
	    /** magic number of a cpio entry in the old binary format */
	    //public const int MAGIC_OLD_BINARY = 070707;
		public const int MAGIC_OLD_BINARY = 0x71C7;
	
	    // These FORMAT_ constants are internal to the code
	    
	    /** write/read a CPIOArchiveEntry in the new format */
	    public const short FORMAT_NEW = 1;
	
	    /** write/read a CPIOArchiveEntry in the new format with crc */
	    public const short FORMAT_NEW_CRC = 2;
	
	    /** write/read a CPIOArchiveEntry in the old ascii format */
	    public const short FORMAT_OLD_ASCII = 4;
	
	    /** write/read a CPIOArchiveEntry in the old binary format */
	    public const short FORMAT_OLD_BINARY = 8;
	
	    /** Mask for both new formats */
	    public const short FORMAT_NEW_MASK = 3;
	
	    /** Mask for both old formats */
	    public const short FORMAT_OLD_MASK = 12;
	
	    /*
	     * Constants for the MODE bits
	     */
	    
	    /** Mask for all file type bits. */
	    public const int S_IFMT   = 0xF000;
	
	 // http://www.opengroup.org/onlinepubs/9699919799/basedefs/cpio.h.html
	 // has a list of the C_xxx constatnts
	
	    /** Defines a socket */
	    public const int C_ISSOCK = 0xC000;
	
	    /** Defines a symbolic link */
	    public const int C_ISLNK  = 0xA000;
	
	    /** HP/UX network special (C_ISCTG) */
	    public const int C_ISNWK  = 0x9000;
	
	    /** Defines a regular file */
	    public const int C_ISREG  = 0x8000;
	
	    /** Defines a block device */
	    public const int C_ISBLK  = 0x6000;
	
	    /** Defines a directory */
	    public const int C_ISDIR  = 0x4000;
	
	    /** Defines a character device */
	    public const int C_ISCHR  = 0x2000;
	
	    /** Defines a pipe */
	    public const int C_ISFIFO = 0x1000;
	
	
	    /** Set user ID */
	    public const int C_ISUID  = 0x800;
	
	    /** Set group ID */
	    public const int C_ISGID  = 0x400;
	
	    /** On directories, restricted deletion flag. */
	    public const int C_ISVTX  = 0x200;
	
	
	    /** Permits the owner of a file to read the file */
	    public const int C_IRUSR  = 0x100;
	
	    /** Permits the owner of a file to write to the file */
	    public const int C_IWUSR  = 0x80;
	
	    /** Permits the owner of a file to execute the file or to search the directory */
	    public const int C_IXUSR  = 0x40;
	
	
	    /** Permits a file's group to read the file */
	    public const int C_IRGRP  = 0x20;
	
	    /** Permits a file's group to write to the file */
	    public const int C_IWGRP  = 0x10;
	
	    /** Permits a file's group to execute the file or to search the directory */
	    public const int C_IXGRP  = 0x8;
	
	
	    /** Permits others to read the file */
	    public const int C_IROTH  = 0000004;
	
	    /** Permits others to write to the file */
	    public const int C_IWOTH  = 0000002;
	
	    /** Permits others to execute the file or to search the directory */
	    public const int C_IXOTH  = 0000001;
		
	 /** The special trailer marker */
	    public const string CPIO_TRAILER = "TRAILER!!!";
	    
		public CpioConstants ()
		{
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