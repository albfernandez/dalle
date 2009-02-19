/*

    Dalle - A split/join file utility library	
    Dalle.Formatos.Axman.CabeceraAxman- Header of axman files.
	
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
			get{
				String ret = version;
				while (ret.Length < 15)
					ret += " ";
				return ret;
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
		
		public static CabeceraAxman LoadFromFile(String file){
			CabeceraAxman cab = new CabeceraAxman();
			
			byte[] bytes = UtilidadesFicheros.LeerSeek(file, 0, 23);
			cab.version = "";
			for (int i=0; i < 15; i++)
				cab.version += Convert.ToChar (bytes[i]);
			cab.fragmento = UtArrays.LeerInt32 (bytes, 15);
			cab.checksum = UtArrays.LeerInt32 (bytes, 19);
			
			return cab;
			
		}
	}
}
