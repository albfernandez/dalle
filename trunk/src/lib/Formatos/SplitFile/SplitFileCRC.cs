/*

	Dalle - A split/join file utility library
	Dalle.Formatos.SplitFile.SplitFileCRC - Checksum used by SplitFile.
	
    Copyright (C) 2003  Alberto Fernández <infjaf00@yahoo.es>

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

namespace Dalle.Formatos.SplitFile
{
	
	public class SplitFileCRC : CRC
	{
		private int value;
		private int contador;
		private int mask = 0x00FFFFFF;
		
		

		public SplitFileCRC ()
		{
			value = 0;
			contador = 0;
		}
		
		/// <summary>
		/// Returns the data checksum computed so far.
		/// </summary>

		public long Value{
			get { return value; }
		}
		
		/// <summary>
		/// Resets the data checksum as if no update was ever called.
		/// </summary>

		public void Reset ()
		{
			value = 0;
			contador = 0;
		}

		/// <summary>
		/// Adds one byte to the data checksum.
		/// </summary>
		/// <param name = "bval">
		/// the data value to add. The high byte of the int is ignored.
		/// </param>

		public void Update (int bval)
		{
			value = (value + contador + bval) & mask;
			contador++;
		}

		/// <summary>
		/// Updates the data checksum with the bytes taken from the array.

		/// </summary>
		/// <param name="buffer">
		/// buffer an array of bytes
		/// </param>

		public void Update (byte[]buffer)
		{
			Update (buffer, 0, buffer.Length);
		}
		/// <summary>
		/// Adds the byte array to the data checksum.
		/// </summary>
		/// <param name = "buf">
		/// the buffer which contains the data
		/// </param>
		/// <param name = "off">
		/// the offset in the buffer where the data starts
		/// </param>
		/// <param name = "len">
		/// the length of the data
		/// </param>

		public void Update (byte[]buf, int off, int len)
		{
			if (buf == null){
				throw new ArgumentNullException ("buf");
			}
			if (off < 0 || len < 0 || off + len > buf.Length){
				throw new ArgumentOutOfRangeException ();
			}

			while (--len >= 0) {
				value = (value + contador + buf[off]) & mask;
				contador++;
				off++;
			}
		}
	}
}
