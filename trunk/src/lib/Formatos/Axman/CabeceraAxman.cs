/*

    Dalle - A split/join file utility library	
    Dalle.Formatos.Axman.CabeceraAxman- Header of axman files.
	
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
using System.Text;
using Dalle.Utilidades;

namespace Dalle.Formatos.Axman
{

	public class CabeceraAxman
	{
		private String version = "AXMAN_03-12r   ";
		private int fragmento = 0;
		private int checksum = -1;

		
		public CabeceraAxman ()
		{
		}
		
		
		public int Checksum{
			get{ return checksum; }
			set{ checksum = value; }
		}
		public int Fragmento{
			get{ return fragmento; }
			set{ fragmento = value; }
		}
		public int Size{
			get{ return 23; }
		}
		public String Version {
			get {
				StringBuilder ret = new StringBuilder (version);
				while (ret.Length < 15)
					ret.Append(" ");
				return ret.ToString();
			}
			set{
				if (value.Length > 15)
					version = value.Substring(0,15);
				else
					version = value;
			}
		}
		
		public byte[] ToByteArray()
		{
			byte[] ret = new byte[23];
			

			UtArrays.EscribirTexto (ret, Version, 0);
			UtArrays.EscribirInt   (ret, (int) Fragmento, 15);
			UtArrays.EscribirInt   (ret, Checksum, 19);
			
			return ret;
		}	
		
		public static CabeceraAxman LoadFromFile (String file)
		{
			byte[] bytes = UtilidadesFicheros.LeerSeek (file, 0, 23);
			return LoadFromArray (bytes);		
		}
		public static CabeceraAxman LoadFromArray (byte[] bytes)
		{
			CabeceraAxman cab = new CabeceraAxman ();
			cab.version = Utilidades.UtArrays.LeerTexto (bytes, 0, 15);
			cab.fragmento = UtArrays.LeerInt32 (bytes, 15);
			cab.checksum = UtArrays.LeerInt32 (bytes, 19);
			return cab;
		}
	}
}
