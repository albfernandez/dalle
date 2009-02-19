/*

	Dalle - A split/join file utility library
	Dalle.Formatos.FileSplit.FileSplit - Join files in FileSplit format.		
	
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
using System.IO;
using Dalle.Formatos;
using Dalle.Utilidades;

namespace Dalle.Formatos.FileSplit 
{
	
	// TODO: terminar la implementacion de esta clase.
	
	public class FileSplit:Parte
	{

		public FileSplit ()
		{
			nombre = "fsi";
			descripcion = "FileSplit v 2.3";
			web = "http://www.partridgesoft.com/filesplit";
			compatible = true;
			parteFicheros = false;
		}

		protected override void _Unir (string fichero, string dirDest)
		{
			
			FileSplit_FSI fsi = FileSplit_FSI.LoadFromFile (fichero);
			string destino = dirDest + Path.DirectorySeparatorChar + fsi.NombreOriginal;
			UtilidadesFicheros.ComprobarSobreescribir (destino);
			long transferidos = 0;
			OnProgress (0,1);
			string fmt = fichero.Substring (0, fichero.Length -4) + ".{0:000}";	
			
			for (int i=0; File.Exists (string.Format (fmt, i)); i++){
				transferidos += 
					UtilidadesFicheros.CopiarTodo (string.Format (fmt, i), destino);
				OnProgress (transferidos, fsi.TamanoOriginal);
			}
		}
		protected override void _Partir (string fichero,string sal1, string dir, long kb)
		{			
		}
		
		public override bool PuedeUnir (string fichero)
		{
			if (! File.Exists (fichero) )
				return false;

			return (fichero.ToUpper().EndsWith(".FSI"));
		}
	}
}
