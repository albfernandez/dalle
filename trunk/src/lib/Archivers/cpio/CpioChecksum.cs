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
using ICSharpCode.SharpZipLib.Checksums;

namespace Dalle.Archivers.cpio
{


	public class CpioChecksum : IChecksum
	{
		byte[] oneByte = new byte[1];
		private long crc = 0;

		#region IChecksum implementation
		public void Reset ()
		{
			this.crc = 0;
		}
		
		public void Update (byte[] buffer)
		{
			Update (buffer, 0, buffer.Length);
		}
		
		public void Update (byte[] buf, int off, int len)
		{
			for (int i = off; i < len; i++)
			{
				 this.crc += buf[i] & 0xFF;
			}
		}
		
		public void Update (int bval)
		{
			oneByte[0] = (byte) (bval & 0xFF);
			Update(oneByte,0,1);
		}
		
		public long Value {
			get {
				return crc;
			}
		}
		#endregion

		public CpioChecksum ()
		{
			this.Reset ();
		}
	}
}

