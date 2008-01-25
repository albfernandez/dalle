/*

	Dalle-sfv-console - A file verification command-line tool.
	Dalle.UI.SFVConsole.GenerateSFVCommand
	
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

namespace Dalle.UI.SFVConsole
{
	public class GenerateSFVCommand : SFVCommand
	{
		private bool recursive;
		private string algorithm;
		public GenerateSFVCommand (string algorithm, bool recursive)
		{
			this.recursive = recursive;
			this.algorithm = algorithm;
		}
		public override void Execute()
		{
			FileVerificationManager.Instance.GenerateSFV (algorithm, files, recursive, Console.Out);					
		}
	}
}
