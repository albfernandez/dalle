/*

	Dalle - A split/join file utility library
	Dalle.Formatos.EasyFileSplitter.EasyFileSpliter - 
		Split and Join files in EasyFileSplitter format.		
	
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

using Dalle.Formatos;
using Dalle.Formatos.Generico;
using Dalle.Utilidades;
using I = Dalle.I18N.GetText;

namespace Dalle.Formatos.EasyFileSplitter
{

	public class EasyFileSplitter : Parte
	{
	
		public EasyFileSplitter()
		{
			nombre = "efsplitter";
			descripcion = "Easy File Splitter v 1.05";
			web = "http://www.filesplitter.net";
			parteFicheros = true;
			compatible = true;			
		}
		
		protected override void _Unir (string fichero, string dirDest)
		{
			InfoEFS info = new InfoEFS (fichero);			
			String original = dirDest + Path.DirectorySeparatorChar + 
					info.NombreOriginal;
			UtilidadesFicheros.ComprobarSobreescribir (original);
			OnProgress (0, info.TotalFragmentos);	
			for (int i=1; i <= info.TotalFragmentos; i++){
				info.Fragmento = i;
				if (!File.Exists (dirDest + Path.DirectorySeparatorChar + info.ToString())){
					string msg = String.Format (I._("File not found: {0}"), info);
					throw new Exception (msg);
				}
				OnProgress (i, info.TotalFragmentos);
			}
			new ParteGenerico().Unir (dirDest + Path.DirectorySeparatorChar + info.Formato, 1, 1);	
		}
		protected override void _Partir (string fichero,string sal1, string dir, long kb)
		{
			
			InfoEFS info = new InfoEFS();
			// Calcular el numero de fragmentos
			long tamano = new FileInfo (fichero).Length;
			long tFragmento = kb * 1024;		
			
			info.NombreOriginal = fichero;
			info.TotalFragmentos = (int) Math.Ceiling ((double) tamano / (double) tFragmento);
			
			String formato = new FileInfo (fichero).Name;
			formato = dir + Path.DirectorySeparatorChar + formato;
			formato += "." + info.TotalFragmentos + "_{0}";			
			
			new ParteGenerico().Partir(fichero, kb,formato, 1,1);
		}
		
		public override bool PuedeUnir (string fichero)
		{
			try {
				InfoEFS i = new InfoEFS (fichero);
				return (i.Fragmento == 1);
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
