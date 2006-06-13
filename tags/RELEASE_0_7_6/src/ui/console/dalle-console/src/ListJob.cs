/*

	Dalle-console - A split/join file utility command-line tool.
	Dalle.UI.Consola.ListJob - Lists all supported formats.
	
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
	
using Dalle.Formatos;
using I = Dalle.I18N.GetText;

namespace Dalle.UI.Consola
{
	
	public class ListJob : IJob
	{
		public ListJob (String[] args)
		{
		}
		public void Ejecutar()
		{

			Console.WriteLine(I._("Join Supported Formats"));
			
			ArrayList l = Manager.GetInstance().GetFormatosUne();
			// TODO: Hacerlo con un String.Format.
			foreach (IParte p in l){
				Console.WriteLine (p.Nombre);
			}
			
			
			Console.WriteLine (I._("Split Supported Formats"));
			l = Manager.GetInstance().GetFormatosParte();
			// TODO: Hacerlo con un String.Format.
			foreach (IParte p in l){
				Console.WriteLine (p.Nombre);
			}		
		}
	}
}
