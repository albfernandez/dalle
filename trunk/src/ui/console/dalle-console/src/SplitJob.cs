/*

	Dalle-console - A split/join file utility command-line tool.
	Dalle.UI.Consola.SplitJob - Split files.
	
    Copyright (C) 2003  Alberto Fern�ndez <infjaf00@yahoo.es>

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

using Dalle.Formatos;
//using I = Dalle.I18N.GetText;

namespace Dalle.UI.Consola
{
	
	
	public class SplitJob : LongJob
	{
		private long tFragmento = 1424;
		private String bas = "";
		private String formato = "";
		private String fichero = "";
		private String dirDest = ".";
		
		public SplitJob (String[] args)
		{

			for (int i=1; i < args.Length; i++){
				
				if (args[i].StartsWith("-")){
					if (args[i].StartsWith ("-t=")){
						tFragmento = Convert.ToInt64 (args[i].Substring (3));
					}
					else if (args[i].StartsWith ("-f=")){
						formato = args[i].Substring (3);
					}
					else if (args[i].StartsWith("-b=")){
						bas = args[i].Substring (3);
					}
					else if (args[i].StartsWith ("-d=")){
						dirDest = args[i].Substring (3);
					}
					else{
						// TODO: Excepci�n personalizada.
						throw new Exception ("0");
					}
				}
				else{
					if (fichero != ""){
						// TODO: Poner una excepci�n personalizada.
						throw new Exception ("1");
					}
					fichero = args[i];
				}
			}
			
			if (fichero == ""){
				// TODO: Excepci�n personalizada.
				throw new Exception ("2");
			}
			
		}
		public override void Ejecutar()
		{
			int inicio = Environment.TickCount;
			Manager.Instance.Progress += new ProgressEventHandler (this.OnProgress);
			OnBegin();
			Manager.GetInstance().Partir(formato, fichero,bas, dirDest, tFragmento);
			OnFinish();
			int final = Environment.TickCount;
			int total = (final - inicio) / 1000;
			this.ShowTotalTime ((final - inicio) / 1000);
		}
		
	}
}
