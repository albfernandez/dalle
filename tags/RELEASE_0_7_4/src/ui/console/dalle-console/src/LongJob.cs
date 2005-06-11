/*

	Dalle-console - A split/join file utility command-line tool.
	Dalle.UI.Consola.LongJob- Shows operation progress.
	
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

	public abstract class LongJob : IJob
	{
		
		private double progreso = 0.0;
		private int asteriscos = 0;
		
		public void OnBegin()
		{
			Console.Write ("[==================================================]\n[");
			asteriscos = 0;
			progreso = 0.0;
		}
		public void OnFinish()
		{
			while (asteriscos < 50){
				Console.Write ("*");
				asteriscos++;
			}
			Console.WriteLine ("]");
			Console.WriteLine (I._("Finished") + ".\n");
		}
		public void OnProgress (long done, long total)
		{
			double fraction = ((double) done) / ((double) total);
			double dif = fraction - progreso;
			progreso = fraction;
			
			// Hemos empezado otra vez.
			// Ej. Hacha pro, con zip.
			
			if (dif < 0) {
				OnFinish();
				OnBegin();
			}
			int astAntes = asteriscos;
			asteriscos = (int) (fraction*50);

			for (int i=0; i < (asteriscos - astAntes); i++)
				Console.Write("*");	
		}
		protected void ShowTotalTime (long seconds)
		{
			Console.WriteLine ( I._("Time = {0} seconds ({1:00}:{2:00})"), 
				seconds, seconds / 60, seconds % 60);
		}
		
		public abstract void Ejecutar ();
	}
}
