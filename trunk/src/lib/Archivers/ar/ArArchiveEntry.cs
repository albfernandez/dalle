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

using Dalle.Archivers;

/**
 * Represents an archive entry in the "ar" format.
 * 
 * Each AR archive starts with "!<arch>" followed by a LF. After these 8 bytes
 * the archive entries are listed. The format of an entry header is as it follows:
 * 
 * <pre>
 * START BYTE   END BYTE    NAME                    FORMAT      LENGTH
 * 0            15          File name               ASCII       16
 * 16           27          Modification timestamp  Decimal     12
 * 28           33          Owner ID                Decimal     6
 * 34           39          Group ID                Decimal     6
 * 40           47          File mode               Octal       8
 * 48           57          File size (bytes)       Decimal     10
 * 58           59          File magic              \140\012    2
 * </pre>
 * 
 * This specifies that an ar archive entry header contains 60 bytes.
 * 
 * Due to the limitation of the file name length to 16 bytes GNU and BSD has
 * their own variants of this format. This formats are currently not supported
 * and file names with a bigger size than 16 bytes are not possible at the
 * moment.
 * 
 */
namespace Dalle.Archivers.ar
{
	public class ArArchiveEntry : ArchiveEntry
	{
    	// The header for each entry 
    	public  const string HEADER = "!<arch>\n";
		// The trailer for each entry 
    	public  const String TRAILER = "`\n";
		
		public const int DEFAULT_MODE = 33188; // = (octal) 0100644 
		
		internal string name;
		private long size;
		private bool isDirectory;
		
		private  int userId;
		private  int groupId;
		private  int mode;
		private  DateTime lastModified;
			
		public ArArchiveEntry (string name, long size) : this(name, size, 0,0,DEFAULT_MODE, new DateTime())
		{
		}
		public ArArchiveEntry (string name, long size, int userId, int groupId, int mode, DateTime lastModified)
		{
			this.name = name;
			this.size = size;
			this.userId = userId;
			this.groupId = groupId;
			this.mode = mode;
			this.lastModified = lastModified;
			this.isDirectory = false;
		}
		public ArArchiveEntry (FileInfo info, string entryName) 
		{
			this.name = entryName;
			this.size = info.Length;
			this.userId = 0;
			this.groupId = 0;
			this.mode = DEFAULT_MODE;
			this.lastModified = info.LastWriteTime;
			this.isDirectory = false;
		}
		
		public bool IsDirectory {
			get { return isDirectory; }
		}

		public string Name {
			get { return name; }
		}

		public long Size {
			get { return size; }
		}

		public int GroupId {
			get {
				return groupId;
			}
		}

		public DateTime LastModified {
			get {
				return lastModified;
			}
		}

		public int Mode {
			get {
				return mode;
			}
		}

		public int UserId {
			get {
				return userId;
			}
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