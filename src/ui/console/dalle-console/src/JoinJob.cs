/*

	Dalle-console - A split/join file utility command-line tool.
	Dalle.UI.ConsolaJoinJob - Joins files.
	
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

	public class JoinJob : LongJob
	{
		private ArrayList lista = new ArrayList();
		private string dirDest = ".";
		
		public JoinJob (String[] args)
		{
			if (args[0] != "-j"){
				lista.Add (args[0]);
			}
			for (int i=1; i < args.Length; i++) {
				if (args[i].StartsWith ("-")){
					if (args[i].StartsWith("-d=")){
						dirDest = args[i].Substring (3);
					}
					else{
						throw new Exception (I._("Bad parameter"));
					}
				}
				lista.Add (args[i]);
			}
		}
		public override void Ejecutar()
		{
			int inicio = Environment.TickCount;
			Manager.Instance.Progress += new ProgressEventHandler (this.OnProgress);
			IParte p;
			foreach (String fichero in lista){
				p = Manager.Instance.GetFormatoFichero (fichero);
				if (p != null){
					Console.WriteLine (I._("Joining file {0} ({1})"),
						fichero, p.Nombre);
					
					OnBegin();
					Manager.Instance.Unir (fichero, dirDest);
					OnFinish();
				}
				else {
					Console.WriteLine (I._("{0}. Error, Unknown format."), fichero);
				}
			}
			int final = Environment.TickCount;
			this.ShowTotalTime ((final - inicio) / 1000);
		}

	}
}
