/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Hacha.NullHachaCRC - 
		Checksum algorithm used by hacha >= 2.79 and HachaPro. 
		Null implementation. Does nothing.			
	
    Copyright (C) 2003-2009  Alberto Fernández <infjaf00@yahoo.es>

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


using Dalle.Checksums;
namespace Dalle.Formatos.Hacha
{
	public class NullHachaCRC : NullCRC
	{
		public NullHachaCRC ()
		{
		}
		public override long Value{
			get{ return 7; }
		}		
	}
}