/*

    Dalle - A split/join file utility library
	
	
	
    Copyright (C) 2010
	Alberto Fernández  <infjaf@gmail.com>

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
using Dalle.Streams;

using ICSharpCode.SharpZipLib.Tar;

namespace Dalle.Archivers.deb
{


	public class DebArchiveEntry : ArchiveEntry
	{

		private string name;
		private long size;
		private bool isDirectory;
		public string Name {
			get { return this.name; }
		}
		public long Size {
			get { return size; }
		}
		public bool IsDirectory {
			get { return this.isDirectory;}
		}
		public DebArchiveEntry (TarEntry t)
		{
			this.name = t.Name;
			this.size = t.Size;
			this.isDirectory = t.IsDirectory;
		}
	}
}
