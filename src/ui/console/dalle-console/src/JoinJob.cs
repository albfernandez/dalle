/*

	Dalle-console - A split/join file utility command-line tool.
	Dalle.UI.ConsolaJoinJob - Joins files.
	
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
						throw new Exception (Catalog.GetString("Bad parameter"));
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
				//Console.WriteLine("Mierda fichero " + fichero);
				//System.Threading.Thread.Sleep(5000);
				p = Manager.Instance.GetFormatoFichero (fichero);
				
				if (p != null){
					Console.WriteLine (Catalog.GetString("Joining file {0} ({1})"), fichero, p.Nombre);					
					OnBegin();
					//Console.WriteLine("OnBegin " + fichero);
					//System.Threading.Thread.Sleep(5000);
					Manager.Instance.Unir (fichero, dirDest);
					//Console.WriteLine("Unir " + fichero);
					//System.Threading.Thread.Sleep(5000);
					OnFinish();
					//Console.WriteLine("Finish " + fichero);
					//System.Threading.Thread.Sleep(5000);
				}
				else {
					Console.WriteLine (Catalog.GetString("{0}. Error, Unknown format."), fichero);
				}
			}
			int final = Environment.TickCount;
			this.ShowTotalTime ((final - inicio) / 1000);
		}

	}
}
