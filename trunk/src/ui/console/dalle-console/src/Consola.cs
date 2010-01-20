/*

	Dalle-console - A split/join file utility command-line tool.
	Dalle.UI.Consola.Consola - A split/join file utility command-line tool.
	
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
	
using Dalle.Formatos;
using Mono.Unix;

namespace Dalle.UI.Consola
{		
	public class Consola
	{		
		public static void Main (String[] args)
		{
			Dalle.I18N.GettextCatalog.Init();
			IJob j = null;
			try{
				j = ProcesarParametros (args);
			}
			catch (Exception e){
				Console.WriteLine (e.Message);
			}

			if (j == null)
				j = new HelpJob(true);

			try{
				j.Ejecutar();
			}
			catch (Exception e){
				Console.WriteLine ("\n\n");
				Console.WriteLine (Catalog.GetString("Error:") + " "  + e.Message);
				Console.WriteLine (e.StackTrace);
			}
		}
		
		private static IJob ProcesarParametros(String[] args)
		{

			if (args.Length == 0)
				return new HelpJob(false);				
			else if (args[0] == "-h" || args[0] == "--help"){
					return new HelpJob(false);
			}
			else if ((args[0] == "--version") || (args[0] == "-v")){
				return new VersionJob();
			}
			else if ((args[0] == "-j") || (args[0] == "--join")){
				return new JoinJob(args);
			}
			else if ((args[0] == "-s") || (args[0] == "--split")){
				return new SplitJob (args);
			}
			else if ((args[0] == "-i") || (args[0] == "--info")){
				return new InfoJob (args);
			}
			else if ((args[0] == "-l") || (args[0] == "--list")) {
				return new ListJob(args);
			}
			else {
				return null;
			} 		
		}	
	}
}
