/*

	Dalle-console - A split/join file utility command-line tool.
	Dalle.UI.Consola.ListJob - Lists all supported formats.
	
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
using System.Collections;
	
using Dalle.Formatos;
using Mono.Unix;

namespace Dalle.UI.Consola
{
	
	public class ListJob : IJob
	{
		public ListJob (String[] args)
		{
		}
		public void Ejecutar()
		{

			Console.WriteLine(Catalog.GetString("Join Supported Formats"));
			
			ArrayList l = Manager.GetInstance().GetFormatosUne();
			// TODO: Hacerlo con un String.Format.
			foreach (IParte p in l){
				Console.WriteLine (p.Nombre);
			}
			
			
			Console.WriteLine (Catalog.GetString("Split Supported Formats"));
			l = Manager.GetInstance().GetFormatosParte();
			// TODO: Hacerlo con un String.Format.
			foreach (IParte p in l){
				Console.WriteLine (p.Nombre);
			}		
		}
	}
}
