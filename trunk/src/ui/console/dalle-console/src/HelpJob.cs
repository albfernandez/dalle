/*

	Dalle-console - A split/join file utility command-line tool.
	Dalle.UI.Consola.HelpJob - Shows help for the application.
	
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
using System.Reflection;

using Mono.Unix;

namespace Dalle.UI.Consola 
{	
	public class HelpJob: IJob
	{
		
		private bool error = false;
		/// <summary>Crea una nueva instancia de la clase.</summary>
		/// <param name="error">Nos indica si se debe a un error
		/// en el formato de los parámetros.</param>

		public HelpJob (bool error)
		{
			this.error = error;
		}

		public void Ejecutar ()
		{
			new VersionJob().Ejecutar();
			if (error){
				Console.WriteLine ("");
				Console.WriteLine (Catalog.GetString("Bad options"));
			}
			Console.WriteLine ("");
			Console.WriteLine (Catalog.GetString("dalle-console <command> [options] <files>"));
			Console.WriteLine ("");
			Console.WriteLine (Catalog.GetString("<commands>"));
			Console.WriteLine (Catalog.GetString("-s, --split Split files"));
			Console.WriteLine (Catalog.GetString("-j, --join  Join files"));
			Console.WriteLine (Catalog.GetString("-i, --info  Show info about files (format)"));
			Console.WriteLine (Catalog.GetString("-l, --list  List all supported formats"));
			Console.WriteLine (Catalog.GetString("(no command) Join files"));
			Console.WriteLine ("");
			Console.WriteLine (Catalog.GetString("[options]"));
			Console.WriteLine (Catalog.GetString("-d=dir "));
			Console.WriteLine (Catalog.GetString("-f=format  Format used to split the file"));
			Console.WriteLine (Catalog.GetString("-b=file Name of dest file, or prefix for fragments"));
			Console.WriteLine (Catalog.GetString("-t=size Size of pieces (in kb)"));

		}
	}
}
