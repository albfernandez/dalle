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
using Dalle.Archivers;



/*
 * A cpio archive consists of a sequence of files. There are several types of
 * headers defided in two categories of new and old format. The headers are
 * recognized by magic numbers:
 * 
 * <ul>
 * <li>"070701" ASCII for new portable format</li>
 * <li>"070702" ASCII for new portable format with CRC format</li>
 * <li>"070707" ASCII for old ascii (also known as Portable ASCII, odc or old
 * character format</li>
 * <li>070707 binary for old binary</li>
 * </ul>
 *
 * <p>The old binary format is limited to 16 bits for user id, group
 * id, device, and inode numbers. It is limited to 4 gigabyte file
 * sizes.
 * 
 * The old ASCII format is limited to 18 bits for the user id, group
 * id, device, and inode numbers. It is limited to 8 gigabyte file
 * sizes.
 * 
 * The new ASCII format is limited to 4 gigabyte file sizes.
 * 
 * CPIO 2.5 knows also about tar, but it is not recognized here.</p>
 * 
 * 
 * <h3>OLD FORMAT</h3>
 * 
 * <p>Each file has a 76 (ascii) / 26 (binary) byte header, a variable
 * length, NUL terminated filename, and variable length file data. A
 * header for a filename "TRAILER!!!" indicates the end of the
 * archive.</p>
 * 
 * <p>All the fields in the header are ISO 646 (approximately ASCII)
 * strings of octal numbers, left padded, not NUL terminated.</p>
 * 
 * <pre>
 * FIELDNAME        NOTES 
 * c_magic          The integer value octal 070707.  This value can be used to deter-
 *                  mine whether this archive is written with little-endian or big-
 *                  endian integers.
 * c_dev            Device that contains a directory entry for this file 
 * c_ino            I-node number that identifies the input file to the file system 
 * c_mode           The mode specifies both the regular permissions and the file type.
 * c_uid            Numeric User ID of the owner of the input file 
 * c_gid            Numeric Group ID of the owner of the input file 
 * c_nlink          Number of links that are connected to the input file 
 * c_rdev           For block special and character special entries, this field 
 *                  contains the associated device number.  For all other entry types,
 *                  it should be set to zero by writers and ignored by readers.
 * c_mtime[2]       Modification time of the file, indicated as the number of seconds
 *                  since the start of the epoch, 00:00:00 UTC January 1, 1970.  The
 *                  four-byte integer is stored with the most-significant 16 bits
 *                  first followed by the least-significant 16 bits.  Each of the two
 *                  16 bit values are stored in machine-native byte order.
 * c_namesize       Length of the path name, including the terminating null byte 
 * c_filesize[2]    Length of the file in bytes. This is the length of the data 
 *                  section that follows the header structure. Must be 0 for 
 *                  FIFOs and directories
 *               
 * All fields are unsigned short fields with 16-bit integer values
 * apart from c_mtime and c_filesize which are 32-bit integer values
 * </pre>
 * 
 * <p>If necessary, the filename and file data are padded with a NUL byte to an even length</p>
 * 
 * <p>Special files, directories, and the trailer are recorded with
 * the h_filesize field equal to 0.</p>
 * 
 * <p>In the ASCII version of this format, the 16-bit entries are represented as 6-byte octal numbers,
 * and the 32-bit entries are represented as 11-byte octal numbers. No padding is added.</p>
 * 
 * <h3>NEW FORMAT</h3>
 * 
 * <p>Each file has a 110 byte header, a variable length, NUL
 * terminated filename, and variable length file data. A header for a
 * filename "TRAILER!!!" indicates the end of the archive. All the
 * fields in the header are ISO 646 (approximately ASCII) strings of
 * hexadecimal numbers, left padded, not NUL terminated.</p>
 * 
 * <pre>
 * FIELDNAME        NOTES 
 * c_magic[6]       The string 070701 for new ASCII, the string 070702 for new ASCII with CRC
 * c_ino[8]
 * c_mode[8]
 * c_uid[8]
 * c_gid[8]
 * c_nlink[8]
 * c_mtim[8]
 * c_filesize[8]    must be 0 for FIFOs and directories 
 * c_maj[8]
 * c_min[8] 
 * c_rmaj[8]        only valid for chr and blk special files 
 * c_rmin[8]        only valid for chr and blk special files 
 * c_namesize[8]    count includes terminating NUL in pathname 
 * c_check[8]       0 for "new" portable format; for CRC format
 *                  the sum of all the bytes in the file
 * </pre>
 * 
 * <p>New ASCII Format The "new" ASCII format uses 8-byte hexadecimal
 * fields for all numbers and separates device numbers into separate
 * fields for major and minor numbers.</p>
 * 
 * <p>The pathname is followed by NUL bytes so that the total size of
 * the fixed header plus pathname is a multiple of four. Likewise, the
 * file data is padded to a multiple of four bytes.</p>
 * 
 * <p>This class uses mutable fields and is not considered to be
 * threadsafe.</p>
 * 
 * <p>Based on code from the jRPM project (http://jrpm.sourceforge.net).</p>
 *
 * <p>The MAGIC numbers and other constants are defined in {@link CpioConstants}</p>
 * 
 * <p>
 * N.B. does not handle the cpio "tar" format
 * </p>
 */

namespace Dalle.Archivers.cpio
{


	public class CpioArchiveEntry :CpioConstants,ArchiveEntry
	{

		private string name;
		private long size;

		public bool IsDirectory {
			get {
				return (this.mode & S_IFMT) == C_ISDIR;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				this.name = value;
			}
		}
		
		public long Size {
			get {
				return size;
			}
			set 
			{
				 if (value < 0 || value > 0xFFFFFFFFL) {
		            throw new InvalidOperationException("invalid entry size <" + value + ">");
		        }
		        this.size = value;
			}
			
		}


	
		private short fileFormat; 
		private int headerSize;
		private int alignmentBoundary;
		private long chksum = 0;
		private long gid = 0;
		private long inode = 0;
		private long maj = 0;
		private long min = 0;
		private long mode = 0;
		private long mtime = 0;
		private long nlink = 0;
		private long rmaj = 0;
		private long rmin = 0;
		private long uid = 0;

		
		 /**
     * Ceates a CPIOArchiveEntry with a specified format.
     * 
     * @param format
     *            The cpio format for this entry.
     * <br/>
     * Possible format values are:
     * <p>
     * CpioConstants.FORMAT_NEW<br/>
     * CpioConstants.FORMAT_NEW_CRC<br/>
     * CpioConstants.FORMAT_OLD_BINARY<br/>
     * CpioConstants.FORMAT_OLD_ASCII<br/>
     * 
     */
    	public CpioArchiveEntry (short format)
    	{
    		switch (format) {
    		case FORMAT_NEW:
    			this.headerSize = 110;
    			this.alignmentBoundary = 4;
    			break;
    		case FORMAT_NEW_CRC:
    			this.headerSize = 110;
    			this.alignmentBoundary = 4;
    			break;
    		case FORMAT_OLD_ASCII:
    			this.headerSize = 76;
    			this.alignmentBoundary = 0;
    			break;
    		case FORMAT_OLD_BINARY:
    			this.headerSize = 26;
    			this.alignmentBoundary = 2;
    			break;
    		default:
    			throw new System.InvalidOperationException ("Unknown header type");
    		}
    		this.fileFormat = format;
    	}
		 /**
     * Ceates a CPIOArchiveEntry with a specified name. The format of this entry
     * will be the new format.
     * 
     * @param name
     *            The name of this entry.
     */
	    public CpioArchiveEntry (String name) : this(FORMAT_NEW)
	    {
	    	this.name = name;
	    }
		 /**
     * Creates a CPIOArchiveEntry with a specified name. The format of this entry
     * will be the new format.
     * 
     * @param name
     *            The name of this entry.
     * @param size
     *            The size of this entry
     */
    	public CpioArchiveEntry (String name, long size) : this(FORMAT_NEW)
    	{
    		this.name = name;
    		this.Size=size;
    	}
		
		public CpioArchiveEntry (FileInfo inputFile, String entryName) : this(entryName, inputFile.Length)
		{
			
			long mode = 0;
			mode |= C_ISREG;
			this.Mode =mode;
			/*
			if (inputFile.isDirectory ()) {
				mode |= C_ISDIR;
			} else if (inputFile.isFile ()) {
				mode |= C_ISREG;
			} else {
				throw new InvalidOperationException ("Cannot determine type of file " + inputFile.getName ());
			}
			// TODO set other fields as needed
			setMode (mode);
			*/
		}

		


		    /**
     * Check if the method is allowed for the defined format.
     */
    private void CheckNewFormat ()
    {
    	if ((this.fileFormat & FORMAT_NEW_MASK) == 0) {
    		throw new NotSupportedException (); 
        }
    }

    /**
     * Check if the method is allowed for the defined format.
     */
    private void CheckOldFormat ()
    {
    	if ((this.fileFormat & FORMAT_OLD_MASK) == 0) {
    		throw new NotSupportedException ();
        }
    }

    /**
     * Get the checksum.
     * Only supported for the new formats.
     * 
     * @return Returns the checksum.
     * @throws UnsupportedOperationException if the format is not a new format
     */
    public long Chksum {
    	get {
    		CheckNewFormat ();
    		return this.chksum;
    	}
    	set {
    		CheckNewFormat ();
    		this.chksum = value;
		}
    }

    /**
     * Get the device id.
     * 
     * @return Returns the device id.
     * @throws UnsupportedOperationException
     *             if this method is called for a CPIOArchiveEntry with a new
     *             format.
     */
    public long Device {
    	get {
    		CheckOldFormat ();
    		return this.min;
    	}
    	set {
    		CheckOldFormat ();
    		this.min = value;		
		}
    }

    /**
     * Get the major device id.
     * 
     * @return Returns the major device id.
     * @throws UnsupportedOperationException
     *             if this method is called for a CPIOArchiveEntry with an old
     *             format.
     */
    public long DeviceMaj {
    	get {
    		CheckNewFormat ();
    		return this.maj;
    	}
    	set	 {
			CheckNewFormat();
	        this.maj = value;
		}
    }

    /**
     * Get the minor device id
     * 
     * @return Returns the minor device id.
     * @throws UnsupportedOperationException if format is not a new format
     */
    public long DeviceMin {
    	get {
    		CheckNewFormat ();
    		return this.min;
    	}
    	set {
			CheckNewFormat();
        	this.min = value;
		}
    }


    /**
     * Get the format for this entry.
     * 
     * @return Returns the format.
     */
    public short Format {
        get {return this.fileFormat;}
    }

    /**
     * Get the group id.
     * 
     * @return Returns the group id.
     */
    public long GID {
    	get { return this.gid; }
    	set { this.gid = value;}
    }

    /**
     * Get the header size for this CPIO format
     * 
     * @return Returns the header size in bytes.
     */
    public int HeaderSize {
        get {return this.headerSize;}
    }

    /**
     * Get the alignment boundary for this CPIO format
     * 
     * @return Returns the aligment boundary (0, 2, 4) in bytes
     */
    public int AlignmentBoundary {
    	get { return this.alignmentBoundary;}
    }

    /**
     * Get the number of bytes needed to pad the header to the alignment boundary.
     * 
     * @return the number of bytes needed to pad the header (0,1,2,3)
     */
    public int HeaderPadCount
	{
    	get {
    		if (this.alignmentBoundary == 0)
    			return 0;
    		int size = this.headerSize + this.name.Length + 1;
    		// Name has terminating null
    		int remain = size % this.alignmentBoundary;
    		if (remain > 0) {
    			return this.alignmentBoundary - remain;
    		}
    		return 0;
		}
    }

    /**
     * Get the number of bytes needed to pad the data to the alignment boundary.
     * 
     * @return the number of bytes needed to pad the data (0,1,2,3)
     */
    public int DataPadCount 
    {
    	get {
    		if (this.alignmentBoundary == 0)
    			return 0;
    		long size = this.Size;
    		int remain = (int)(size % this.alignmentBoundary);
    		if (remain > 0) {
    			return this.alignmentBoundary - remain;
    		}
    		return 0;
		}
    }

    /**
     * Set the inode.
     * 
     * @return Returns the inode.
     */
    public long Inode {
    	get { return this.inode; }
    	set { this.inode = value;}
    }

    /**
     * Get the mode of this entry (e.g. directory, regular file).
     * 
     * @return Returns the mode.
     */
    public long Mode {
    	get { return this.mode; }
    	set {
				long maskedMode = value & S_IFMT;
			    	switch ((int)maskedMode) {
			    	case C_ISDIR:
			    	case C_ISLNK:
			    	case C_ISREG:
			    	case C_ISFIFO:
			    	case C_ISCHR:
			    	case C_ISBLK:
			    	case C_ISSOCK:
			    	case C_ISNWK:
			    		break;
			    	default:
			    		throw new InvalidOperationException (
			                    "Unknown mode. "
			                    );
								//+ "Full: " + Long.toHexString(mode) 
			                    //+ " Masked: " + Long.toHexString(maskedMode)
			        }
			
			        this.mode = value;
			}
    }



    /**
     * Get the number of links.
     * 
     * @return Returns the number of links.
     */
    public long NumberOfLinks {
    	get { return this.nlink; }
    	set { this.nlink = value;}
    }

    /**
     * Get the remote device id.
     * 
     * @return Returns the remote device id.
     * @throws UnsupportedOperationException
     *             if this method is called for a CPIOArchiveEntry with a new
     *             format.
     */
    public long RemoteDevice {
    	get {
    		CheckOldFormat ();
    		return this.rmin;
    	}
    	set {
    		CheckOldFormat ();
    		this.rmin = value;
		}
    }

    /**
     * Get the remote major device id.
     * 
     * @return Returns the remote major device id.
     * @throws UnsupportedOperationException
     *             if this method is called for a CPIOArchiveEntry with an old
     *             format.
     */
    public long RemoteDeviceMaj 
    {
    	get {
    		CheckNewFormat ();
    		return this.rmaj;
    	}
    	set {
    		CheckNewFormat ();
    		this.rmaj = value;
		}
    }

    /**
     * Get the remote minor device id.
     * 
     * @return Returns the remote minor device id.
     * @throws UnsupportedOperationException
     *             if this method is called for a CPIOArchiveEntry with an old
     *             format.
     */
    public long RemoteDeviceMin 
    {
    	get {
    		CheckNewFormat ();
    		return this.rmin;
    	}
    	set {
    		CheckNewFormat ();
    		this.rmin = value;
		}
    }

    /**
     * Get the time in seconds.
     * 
     * @return Returns the time.
     */
    public long Time {
    	get { return this.mtime; }
    	set { this.mtime = value;}
    }

    /**
     * Get the user id.
     * 
     * @return Returns the user id.
     */
    public long UID {
    	get { return this.uid; }
    	set {this.uid = value;}
    }

    /**
     * Check if this entry represents a block device.
     * 
     * @return TRUE if this entry is a block device.
     */
    public bool IsBlockDevice {
    	get { return (this.mode & S_IFMT) == C_ISBLK;}
    }

    /**
     * Check if this entry represents a character device.
     * 
     * @return TRUE if this entry is a character device.
     */
    public bool IsCharacterDevice {
    	get { return (this.mode & S_IFMT) == C_ISCHR;}
    }


    /**
     * Check if this entry represents a network device.
     * 
     * @return TRUE if this entry is a network device.
     */
    public bool IsNetwork {
    	get { return (this.mode & S_IFMT) == C_ISNWK;}
    }

    /**
     * Check if this entry represents a pipe.
     * 
     * @return TRUE if this entry is a pipe.
     */
    public bool IsPipe {
    	get { return (this.mode & S_IFMT) == C_ISFIFO;}
    }

    /**
     * Check if this entry represents a regular file.
     * 
     * @return TRUE if this entry is a regular file.
     */
    public bool IsRegularFile {
    	get { return (this.mode & S_IFMT) == C_ISREG;}
    }

    /**
     * Check if this entry represents a socket.
     * 
     * @return TRUE if this entry is a socket.
     */
    public bool IsSocket {
    	get { return (this.mode & S_IFMT) == C_ISSOCK;}
    }

    /**
     * Check if this entry represents a symbolic link.
     * 
     * @return TRUE if this entry is a symbolic link.
     */
    public bool IsSymbolicLink {
        get {return (this.mode & S_IFMT) == C_ISLNK;}
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