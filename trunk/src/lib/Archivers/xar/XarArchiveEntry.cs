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
using Dalle.Archivers;

namespace Dalle.Archivers.xar
{


	public class XarArchiveEntry : ArchiveEntry
	{
		#region ArchiveEntry implementation
		public string Name {
			get {
				return this.name;
			}
			set {
				this.name = value;
			}
		}
		
		
		public long Size {
			get {
				return this.size;
			}
			set {
				this.size = value;
			}
		}
		
		
		public bool IsDirectory {
			get {
				return isDirectory;
			}
			set {
				this.isDirectory = value;
			}
		}
		
		#endregion
		public string User {
			get {
				return user;
			}
			set {
				user = value;
			}
		}
		
		
		public int Uid {
			get {
				return uid;
			}
			set {
				uid = value;
			}
		}
		
		
		public string Type {
			get {
				return type;
			}
			set {
				type = value;
			}
		}
		
		
		public long Offset {
			get {
				return offset;
			}
			set {
				offset = value;
			}
		}
		
		
		public string Mode {
			get {
				return mode;
			}
			set {
				mode = value;
			}
		}
		
		
		public long Length {
			get {
				return length;
			}
			set {
				length = value;
			}
		}
		
		
		public long Id {
			get {
				return id;
			}
			set {
				id = value;
			}
		}
		
		
		public string Group {
			get {
				return group_;
			}
			set {
				group_ = value;
			}
		}
		
		
		public int Gid {
			get {
				return gid;
			}
			set {
				gid = value;
			}
		}
		
		
		public string ExtractedChecksum {
			get {
				return extractedChecksum;
			}
			set {
				extractedChecksum = value;
			}
		}
		
		
		public XarEncoding Encoding {
			get {
				return encoding;
			}
			set {
				encoding = value;
			}
		}
		
		
		public string ArchivedChecksum {
			get {
				return archivedChecksum;
			}
			set {
				archivedChecksum = value;
			}
		}
		
		public XarHashAlgorithm HashAlgorithmExtracted {
			get {
				return hashAlgorithmExtracted;
			}
			set {
				hashAlgorithmExtracted = value;
			}
		}
		
		
		public XarHashAlgorithm HashAlgorithmArchived {
			get {
				return hashAlgorithmArchived;
			}
			set {
				hashAlgorithmArchived = value;
			}
		}
		
		
		

		
		
		
		private long id;
		private bool isDirectory;
		private string name;
		private long offset;
		private long length;
		private long size;
		private XarEncoding encoding;
		private XarHashAlgorithm hashAlgorithmArchived;
		private string archivedChecksum;
		private XarHashAlgorithm hashAlgorithmExtracted;
		private string extractedChecksum;
		private int gid;
		private int uid;
		private string user;
		private string group_;
		private string mode;
		private string type;
		
		public XarArchiveEntry ()
		{
			this.encoding = XarEncoding.None;
		}
	}
}
