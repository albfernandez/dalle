/*

	Dalle-console - A split/join file utility command-line tool.
	Dalle.UI.Consola.VersionJob - Shows file format info.
	
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
using System.Collections;
using System.IO;
	
using Dalle.Formatos;
using I = Dalle.I18N.GetText;

namespace Dalle.UI.Consola
{	
	public class InfoJob : IJob
	{

		private ArrayList ficheros = new ArrayList();
		
		public InfoJob (String[] args)
		{
			for (int i=1; i < args.Length; i++) {
				if (args[i].StartsWith ("-")){
					throw new Exception ();
				}
				ficheros.Add (args[i]);
			}

		}
		public void Ejecutar()
		{
			foreach (String fic in ficheros){
				Console.Write ("{0,-50} ", fic);
				if (File.Exists (fic)){
					IParte p = Manager.GetInstance().GetFormatoFichero(fic);
					if (p!=null){
						Console.WriteLine (p.Descripcion);
					}
					else
						Console.WriteLine (I._("Unknown format"));
				}
				else {
					if (Directory.Exists (fic))
						Console.WriteLine(I._("Is a directory"));
					else
						Console.WriteLine (I._("File not found"));
				}
				
			}
		}
		
		
	}
}
