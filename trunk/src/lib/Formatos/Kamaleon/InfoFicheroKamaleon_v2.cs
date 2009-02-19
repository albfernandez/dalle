/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Kamaleon.InfoFicheroKamaleon2	
	
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

using System;

using Dalle.Utilidades;

namespace Dalle.Formatos.Kamaleon
{
	
	public class InfoFicheroKamaleon_v2 : InfoFicheroKamaleon
	{
		public InfoFicheroKamaleon_v2 ()
		{
		}
		public InfoFicheroKamaleon_v2 (byte[] bytes) : base(bytes)
		{
			checksum = UtArrays.LeerInt64(bytes, 0x1C0);
			nombrePiel = GetText (bytes, 0x1C9);
			
			Password = "";
			for (int i=0x105; bytes[i] != 0; i++)
				Password += Convert.ToChar(255 - bytes[i]);
		}
		public override byte[] ToByteArray ()
		{
			byte[] ret = base.ToByteArray();
			for (int i=0; i < Password.Length; i++)
				ret[0x105 + i] = (byte )(255 - Convert.ToByte (Password[i]));			
			
			ret[0x1C8] = (byte) NombrePiel.Length;
			SetText (ret, NombrePiel, 0x1C9);
			UtArrays.EscribirInt (ret, this.Checksum, 0x1C0);
			return ret;			
		}		
	}
}
