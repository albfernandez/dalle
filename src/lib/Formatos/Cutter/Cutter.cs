/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Cutter.Cutter - Basic support for cutter files.
	
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

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;	

using Dalle.Utilidades;

namespace Dalle.Formatos.Cutter
{	
	public class Cutter : Parte 
	{
		//private static int buffSize = 262144;
		
		public Cutter()
		{
			nombre = "cutter";
			descripcion = "Fichero Cutter";
			web = "http://gcutter.free.fr";
			compatible = false;
			parteFicheros = true;
			
		}
		
		protected override void _Unir (string fichero, string dirDest)
		{
			CutterTail tail = CutterTail.LoadFromFile(fichero);
			string destino = dirDest + Path.DirectorySeparatorChar + tail.Original;
			UtilidadesFicheros.ComprobarSobreescribir(destino);
			
		}
		protected override void _Partir (string fichero,string salida1, string dir, long kb)
		{

		}
		public override bool PuedeUnir (string fichero)
		{
			if ( ! File.Exists (fichero) )
				return false;
			if (!fichero.ToUpper().EndsWith (".CUT1"))
				return false;
			CutterTail tail = CutterTail.LoadFromFile (fichero);
			return (tail != null);
		}	
	}
}
