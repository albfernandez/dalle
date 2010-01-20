/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Hacha.HachaCRC - 
		Checksum algorithm used by hacha >= 2.79 and HachaPro		
	
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

using Dalle.Checksums;

namespace Dalle.Formatos.Hacha
{
	
	public class HachaCRC : CRC
	{
		
		private int i1 = 0;
		private int i2 = 0;
		private long invpos = 0;
		private long tFichero = 0;
		
		
		public HachaCRC(long tamanoFichero)
		{
			invpos = tamanoFichero;
			tFichero = tamanoFichero;
		}
		public long Value{
			get{
				if ((tFichero - invpos) <= 1)
					return 0;
				int ret = unchecked ( (i1 << 16) + i2);
				return (ret & 0x00000000FFFFFFFF);
			}
		}
		public void Reset()
		{
			i1 = 0;
			i2 = 0;
			invpos = tFichero; 
		}
		public void Update (int bval)
		{

			int v = (int) invpos;
			v *= bval;
			i1 += v;
			if (i1 > 0xFFF1)
				i1 %= 0xFFF1;			
			i2 += bval;
			if (i2 > 0xFFF1)
				i2 %= 0xFFF1;
			invpos--;
		}
		public void Update (byte[] buf)
		{
			Update (buf, 0, buf.Length);
		}
		public void Update (byte[] buf, int off, int len)
		{
			if (buf == null)
				throw new ArgumentNullException ("buf");

			if (off < 0 || len < 0 || off + len > buf.Length)
				throw new ArgumentOutOfRangeException ();

			while (--len >= 0){
				
				int v = (int) invpos;
				v *= buf[off];
				
				i1+= v;
				i1 %=  0xFFF1;

				i2 += buf[off];
				i2 &= 0x000000FFFFFF;
				i2 %= 0xFFF1;
				invpos--;
				off++;

			}
		}
	}
}
