/*

	Dalle-sfv-console - A file verification command-line tool.
	Dalle.UI.SFVConsole.DalleSFVConsole
	
    Copyright (C) 2003-2009  Alberto Fernández <infjaf00@yahoo.es>

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
using System.IO;

using Dalle.FileVerification;

using Mono.Unix;



namespace Dalle.UI.SFVConsole
{
	public class DalleSFVConsole
	{		
		public static void Main (string[] args)
		{
			Dalle.I18N.GettextCatalog.Init();
			Command c;
			try{
				c = CommandFactory.CreateCommand (args);
			}
			catch (Exception e){
				Console.WriteLine (e.Message);
				new HelpCommand().Execute();
				return;
			}
			c.Execute();				
		}
	
	}
}
