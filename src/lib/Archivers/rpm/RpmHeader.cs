/*

    Dalle - A split/join file utility library
	
	
	
    Copyright (C) 2004-2010
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
using Dalle.Utilidades;

namespace Dalle.Archivers.rpm
{



	public class RpmHeader {		
		int magic;
		int version;
		int numIndex;
		int numData;
		public int Version {
			get {
				return version;
			}
			set {
				version = value;
			}
		}
		
		
		public int NumIndex {
			get {
				return numIndex;
			}
			set {
				numIndex = value;
			}
		}
		
		
		public int NumData {
			get {
				return numData;
			}
			set {
				numData = value;
			}
		}
		
		
		public int Magic {
			get {
				return magic;
			}
			set {
				magic = value;
			}
		}
		
		
		public RpmHeader ()
		{

		}
		public RpmHeader (byte[] data)
		{

			if (data[0] != 0x8E || data[1] != 0xAD || data[2] != 0xE8){
				throw new IOException("Invalid header magic");
			}
			this.numIndex = UtArrays.LeerInt32BE (data, 8);
			this.numData = UtArrays.LeerInt32BE (data, 12);
		}

	}
	
}
