/*

	Dalle-sfv-console - A file verification command-line tool.
	Dalle.UI.SFVConsole.HelpCommand
	
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

using Dalle.FileVerification;
using Mono.Unix;

namespace Dalle.UI.SFVConsole
{
	public class HelpCommand : Command
	{
		public void Execute ()
		{
			Console.WriteLine (Catalog.GetString("Usage:"));
			Console.WriteLine ("");
			Console.WriteLine (Catalog.GetString("To verify files:"));
			Console.WriteLine (Catalog.GetString("dalle-sfv-console -v <md5 and sfv-files>"));
			Console.WriteLine (Catalog.GetString("dalle-sfv-console -vv <md5 and sfv-files>  to be verbose"));
			Console.WriteLine ("");
			Console.WriteLine (Catalog.GetString("To create (md5/sfv) files:"));
			Console.WriteLine (Catalog.GetString("dalle-sfv-console -<format> <files>"));
			Console.WriteLine (Catalog.GetString("dalle-sfv-console -r -<format> <files> to recurse into directories"));
			Console.WriteLine ("");			
			Console.WriteLine (Catalog.GetString("Supported formats: "));
			foreach (IFVerification f in FileVerificationManager.Instance.GetSupportedFormats()){
				Console.WriteLine ("" + f.Name);
			}
			
			Console.WriteLine ("");
			Console.WriteLine (Catalog.GetString("Examples:"));
			Console.WriteLine ("");
			Console.WriteLine (Catalog.GetString("Verifying files with a sfv file"));
			Console.WriteLine ("dalle-sfv-console -vv file.sfv");
			Console.WriteLine ("");
			Console.WriteLine (Catalog.GetString("Creating sfv file (recursing into directories)"));
			Console.WriteLine ("dalle-sfv-console -r -sfv * > file.sfv");
			Console.WriteLine ("");
			
		}
	}
}
