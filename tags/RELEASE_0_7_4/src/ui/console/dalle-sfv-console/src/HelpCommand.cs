/*

	Dalle-sfv-console - A file verification command-line tool.
	Dalle.UI.SFVConsole.HelpCommand
	
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

using Dalle.FileVerification;
using I = Dalle.I18N.GetText;

namespace Dalle.UI.SFVConsole
{
	public class HelpCommand : Command
	{
		public void Execute ()
		{
			Console.WriteLine (I._("Usage:"));
			Console.WriteLine ("");
			Console.WriteLine (I._("To verify files:"));
			Console.WriteLine (I._("dalle-sfv-console -v <md5 and sfv-files>"));
			Console.WriteLine (I._("dalle-sfv-console -vv <md5 and sfv-files>  to be verbose"));
			Console.WriteLine ("");
			Console.WriteLine (I._("To create (md5/sfv) files:"));
			Console.WriteLine (I._("dalle-sfv-console -<format> <files>"));
			Console.WriteLine (I._("dalle-sfv-console -r -<format> <files> to recurse into directories"));
			Console.WriteLine ("");			
			Console.WriteLine (I._("Supported formats: "));
			foreach (IFVerification f in FileVerificationManager.Instance.GetSupportedFormats()){
				Console.WriteLine ("" + f.Name);
			}
			
			Console.WriteLine ("");
			Console.WriteLine (I._("Examples:"));
			Console.WriteLine ("");
			Console.WriteLine (I._("Verifying files with a sfv file"));
			Console.WriteLine ("dalle-sfv-console -vv file.sfv");
			Console.WriteLine ("");
			Console.WriteLine (I._("Creating sfv file (recursing into directories)"));
			Console.WriteLine ("dalle-sfv-console -r -sfv * > file.sfv");
			Console.WriteLine ("");
			
		}
	}
}
