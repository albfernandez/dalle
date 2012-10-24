/*

	Dalle-console - A split/join file utility command-line tool.
	Dalle.UI.Consola.VersionJob - Shows the application version.
	
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
using System.Reflection;

using Mono.Unix;

namespace Dalle.UI.Consola
{
	public class VersionJob : IJob
	{
	
		public VersionJob()
		{
		}
		
		public void Ejecutar ()
		{
			Console.WriteLine (Catalog.GetString("dalle-console v.{0}"), 
				Assembly.GetExecutingAssembly ().GetName ().Version);
			Console.WriteLine (Catalog.GetString("Using libDalle v.{0}"), 
				Dalle.Formatos.Manager.Version);
		}
	}
}
