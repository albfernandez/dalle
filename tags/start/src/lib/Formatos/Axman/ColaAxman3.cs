/*

    Dalle - A split/join file utility library	
    Dalle.Formatos.Axman.ColaAxman3 - Tail of axman3 files.
	
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
using System.IO;

using Dalle.Utilidades;

namespace Dalle.Formatos.Axman
{
	public class ColaAxman3 : ColaAxman
	{
		public ColaAxman3()
		{
		}
		public override int Size{
			get{ return (39 + 2 + FicheroOriginal.Length); }
		}
		public override byte[] ToByteArray()
		{

			byte[] bytes = new byte[this.Size];
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
			UtArrays.EscribirInt (bytes, TamanoOriginal, pos);
			pos+=8;
			UtArrays.EscribirTexto(bytes, "|", pos++);
			UtArrays.EscribirInt (bytes, FicheroOriginal.Length + 2, pos);			
			
			return bytes;
		}
		public static new ColaAxman LoadFromFile (String file)
		{
			byte[] b = UtilidadesFicheros.LeerSeek (file, new FileInfo(file).Length -4, 4);
			int tRuta = UtArrays.LeerInt32 (b, 0);
			
			b = UtilidadesFicheros.LeerSeek(file, new FileInfo(file).Length - (tRuta + 39), tRuta + 39);
			
			if (b[0] != SEPARADOR)
				throw new Exception ("");
			
			ColaAxman3 c = new ColaAxman3();
			
			c.fichOriginal = "";
			int i=0;
			for (i = 1; b[i] != SEPARADOR; i++){
				c.fichOriginal += Convert.ToChar(b[i]);
			}
			i++;
			c.version = "";
			for ( ; b[i] != SEPARADOR; i++){
				c.version += Convert.ToChar(b[i]);
			}
			i++;
			
			// Leemos la versión
			c.ver = UtArrays.LeerInt32 (b, i);
			i+=4;
			if (b[i] != SEPARADOR)
				throw new Exception();
			i++;
			
			// Leemos el fragmento
			c.fragmentos = UtArrays.LeerInt32(b, i);
			i+=4;
			if (b[i] != SEPARADOR)
				throw new Exception();
			i++;
			
			c.tamanoOriginal = UtArrays.LeerInt64(b, i);
			i+=8;
			if (b[i] != SEPARADOR)
				throw new Exception ();
			
			
			return c;
		}
	}
}
