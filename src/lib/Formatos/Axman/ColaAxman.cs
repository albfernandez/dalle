/*

    Dalle - A split/join file utility library	
    Dalle.Formatos.Axman.ColaAxman - Tail of axman files.
	
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
	// TODO: Terminar
	public class ColaAxman
	{

		protected const byte SEPARADOR = 0x7C;
		protected string fichOriginal;
		protected string version;
		protected int ver;
		protected int fragmentos;
		protected long tamanoOriginal;

		public String FicheroOriginal{
			get{return fichOriginal;}
			set{fichOriginal = value;}
		}
		public String Version{
			get{
				String ret = version;
				while (ret.Length < 15)
					ret+=" ";
				return ret;
			}
			set{
				if (value.Length > 15)
					version = value.Substring(0,15);
				else
					version = value;
			}
		}
		public int Ver{
			get{return ver;}
			set{ver = value;}
		}
		public int Fragmentos{
			get{return fragmentos;}
			set{fragmentos = value;}
		}
		public long TamanoOriginal{
			get{return tamanoOriginal;}
			set{tamanoOriginal = value;}
		}
		public String Nombre{
			get{
				return FicheroOriginal.Substring  (FicheroOriginal.LastIndexOf('\\')+1);
			}
		}
		public virtual int Size{
			get{ return (35 + 2 + FicheroOriginal.Length); }
		}

		public ColaAxman (){
		}
		
		public virtual byte[] ToByteArray(){

			byte[] bytes = new byte[Size];
			int pos = 0;
			UtArrays.EscribirTexto (bytes, "|" + FicheroOriginal + "|", 0);
			pos += FicheroOriginal.Length + 2;
			
			UtArrays.EscribirTexto (bytes, Version + "|", pos);
			pos+=16;
			UtArrays.EscribirInt (bytes, (int) 2, pos);
			pos+=4;
			UtArrays.EscribirTexto(bytes, "|", pos++);
			UtArrays.EscribirInt (bytes, Fragmentos, pos);
			pos+=4;
			UtArrays.EscribirTexto(bytes, "|", pos++);
			UtArrays.EscribirInt (bytes, (int) TamanoOriginal, pos);
			pos+=4;
			UtArrays.EscribirTexto(bytes, "|", pos++);
			UtArrays.EscribirInt (bytes, FicheroOriginal.Length + 2, pos);
			
			return bytes;
		}
		
		public static ColaAxman NewFromVersion (int version)
		{
			switch (version){
				case 3: return new ColaAxman3();
				default: return new ColaAxman();
			}
		}
		public static ColaAxman LoadFromFile (String fichero)
		{
			//TODO:Completar la funci�n.
			return new ColaAxman();
		}
	}
}
