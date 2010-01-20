/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Hacha.CabeceraHacha_v2 - 
		Header of files created by Hacha >= 2.79
	
    Copyright (C) 2003-2010  Alberto Fernández <infjaf@gmail.com>

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

namespace Dalle.Formatos.Hacha
{
	
	
	public class CabeceraHacha_v2 : CabeceraHacha_v1
	{
		
		public CabeceraHacha_v2()
		{
		}
		public override string Version{
			get{ return "2"; }
		}
		public override byte[] ToByteArray()
		{			
			byte[] ret = base.ToByteArray();
					
			ret[5] = (byte) (CRC & 0x000000FF);
			ret[6] = (byte) ((CRC & 0x0000FF00) >> 8);
			ret[7] = (byte) ((CRC & 0x00FF0000) >> 16);
			//ret[8] = (byte) (unchecked ((CRC & (int)0xFF000000)>>24));
			ret[8] = (byte) (CRC >> 24);
			return ret;
		}
	}
}
