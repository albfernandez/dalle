/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Kamaleon/Kamaleon2CRC - 
		Checksum algorithm used by KamaleoN2
	
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

namespace Dalle.Formatos.Astrotite {

	public class AstrotiteCRC : CRC {
		int crc;
		
		public long Value {
			get { return (long) crc; }
		}
		
		public AstrotiteCRC(){
			Reset();
		}
		public void Reset(){
			crc = 0;
		}
		public void Update (int bval){
			bval &= 0xFF;
			crc +=  bval;
		}
		public void Update (byte[] buf){
			Update (buf, 0, buf.Length);
		}
		public void Update (byte[] buf, int off, int len){
			if (buf == null){
				throw new ArgumentNullException ("buf");
			}
			if (off < 0 || len < 0 || off + len > buf.Length){
				throw new ArgumentOutOfRangeException ();
			}
			while (--len >= 0){
				crc += buf[off];	
				off++;				
			}
		}
	}
}
