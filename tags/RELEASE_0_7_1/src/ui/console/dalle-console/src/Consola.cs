/*

	Dalle-console - A split/join file utility command-line tool.
	Dalle.UI.Consola.Consola - A split/join file utility command-line tool.
	
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
	
using Dalle.Formatos;
using I = Dalle.I18N.GetText;

namespace Dalle.UI.Consola
{		
	public class Consola
	{		
		public static void Main (String[] args)
		{
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
				Console.WriteLine ("Error: " + e.Message);
			}
		}
		
		private static IJob ProcesarParametros(String[] args)
		{

			if (args.Length == 0)
				return new HelpJob(false);				
			else if (args[0] == "-h" || args[0] == "--h" || args[0] == "-help" ||
				args[0] == "--help"){
					return new HelpJob(false);
			}
			else if (args[0] == "--version")
				return new VersionJob();
			else if (! args[0].StartsWith ("-") )
				return new JoinJob (args);
			else if (args[0] == "-j")
				return new JoinJob(args);
			else if (args[0] == "-s")
				return new SplitJob (args);
			else if (args[0] == "-i")
				return new InfoJob (args);
			else if (args[0] == "-l")
				return new ListJob(args);
			else
				return null; 		
		}	
	}
}
