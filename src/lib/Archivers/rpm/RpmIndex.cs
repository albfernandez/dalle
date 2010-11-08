/*

    Dalle - A split/join file utility library
	
	
	
    Copyright (C) 2004-2010 Alberto Fernández  <infjaf@gmail.com>
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

namespace Dalle.Archivers.rpm
{



	public class RpmIndex
	{
		private RpmType type;
		private int offset;
		private int count;

		public RpmType Type {
			get { return type; }
			set { type = value; }
		}
		public int Offset {
			get { return offset; }
			set { offset = value; }
		}


		public int Count {
			get { return count; }
			set { count = value; }
		}
		
	}
}
