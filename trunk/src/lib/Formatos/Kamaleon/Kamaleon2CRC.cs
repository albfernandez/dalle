/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Kamaleon/Kamaleon2CRC - 
		Checksum algorithm used by KamaleoN2
	
    Copyright (C) 2004
    Original author (C - code) - Dai SET <dai_set@yahoo.com>
    C# translation by - Alberto Fernández  <infjaf00@yahoo.es>

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

*/

using System;

using Dalle.Checksums;

namespace Dalle.Formatos.Kamaleon
{
	
	public class Kamaleon2CRC : CRC {
	
		private int contador = 0;
		private long crc = 0;
		
		
		
		public long Value {
			get {
				double dcrc  = (double) crc;
				return BitConverter.DoubleToInt64Bits(dcrc);
			}
		}
		public Kamaleon2CRC ()
		{
			Reset ();
		}
		public void Reset ()
		{
			contador = 0;
			crc = 0;
		}
		public void Update (int bval)
		{
			bval &= 0xFF;
			crc += Math.Abs(bval - contador);
			++contador;
			contador %= 255;
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
				crc += Math.Abs(buf[off] - contador);
				off++;
				++contador;
				contador %= 255;
			}
		}
	
	
	}
}
