/*

	Dalle - A split/join file utility library
	Dalle.Formatos.FileSplit.FileSplit_FSI - FSI file format support.		
	
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
	

namespace Dalle.Formatos.FileSplit
{
	// TODO: Terminar (ahora sólo lee unos pocos campos).
	public class FileSplit_FSI
	{
		private long tFichero;		
		private string nombreOriginal;
		private string comentario;
	
		private FileSplit_FSI()
		{
		}
		
		public long TamanoOriginal{
			get { return tFichero; }
		}
		public string NombreOriginal{
			get { return nombreOriginal; }
		}
		public string Comentario {
			get { return comentario; }
		}
	
		// TODO: Mejorar la detección del formato.
		public static FileSplit_FSI LoadFromFile(string fichero)
		{
			FileSplit_FSI fsi = new FileSplit_FSI();
			
			byte[] contenido = UtilidadesFicheros.LeerTodo (fichero);			
			
			fsi.tFichero = UtArrays.LeerInt32(contenido, 0x28);
			
			int longComentario = UtArrays.LeerInt32(contenido, 0x30);
			int longNombre = UtArrays.LeerInt32(contenido, 0x34);
			
			fsi.nombreOriginal = "";
			int i = 0x38;
			for (i=0x38; i < (0x38 + longNombre); i++){
				fsi.nombreOriginal += Convert.ToChar(contenido[i]);
			}
			
			fsi.comentario = "";
			for ( ; i < (0x38 + longNombre + longComentario); i++){
				fsi.comentario += Convert.ToChar(contenido[i]);
			}		
			
			return fsi;
		}
	}
	
}