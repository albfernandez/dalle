/*

    Dalle - A split/join file utility library
    Dalle.Formatos.Astrotite.Astrotite.v0.AstrotiteV0InputStream
          Join files in astrotite format (version 0.02).
	
    Copyright (C) 2004-2010
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

namespace Dalle.Formatos.Astrotite.v0
{


	public class AstrotiteV0Entry : ArchiveEntry
	{

		private long size;
		private string name;
		#region ArchiveEntry implementation
		public bool IsDirectory {
			get {
				return false;
			}
		}
		
		
		public string Name {
			get {
				return name;
			}
		}
		
		
		public long Size {
			get {
				return this.size;
			}
			internal set { this.size = value; }
		}

		
		#endregion
		public AstrotiteV0Entry (string name, long size)
		{
			this.name = name;
			this.size = size;		
		}
		
	}
}
