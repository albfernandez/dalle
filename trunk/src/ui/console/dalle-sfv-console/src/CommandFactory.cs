/*

	Dalle-sfv-console - A file verification command-line tool.
	Dalle.UI.SFVConsole.CommandFactory 
	
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
using System.IO;

namespace Dalle.UI.SFVConsole
{
	public class CommandFactory 
	{
		public static Command CreateCommand (string[] args)
		{
			if (args.Length < 1)
				return new HelpCommand();
			
			if (args[0] == "-v" || args[0] == "-vv"){
				VerifySFVCommand ret = new VerifySFVCommand (args[0] == "-vv" ? true : false);
				for (int i1 = 1; i1 < args.Length; i1++){
					if (!File.Exists (args[i1]))
						throw new Exception ("");
					ret.files.Add (args[i1]);
				}
				return ret;
			}
			if (args[0] == "-help" || args[0] == "--help")
				return new HelpCommand();
			
			GenerateSFVCommand ret2;
			int i = 0;
			bool recursive = false;
			string algorithm;
			if (args[0] == "-r"){
				recursive = true;
				i++;
			}
			if (args[i].StartsWith("-")){
				algorithm = args[i].Substring(1);
				i++;
			}
			else
				algorithm = "md5";
			
			ret2 = new GenerateSFVCommand (algorithm, recursive);
			
			for ( ; i < args.Length; i++){
				if (!File.Exists (args[i]) && !Directory.Exists(args[i]))
					throw new Exception ("---");
				ret2.files.Add (args[i]);
			}
			return ret2;
		}
	}
}
