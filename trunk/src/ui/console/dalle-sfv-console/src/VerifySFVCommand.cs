/*

	Dalle-sfv-console - A file verification command-line tool.
	Dalle.UI.SFVConsole.VerifySFVCommand
	
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
	public class VerifySFVCommand : SFVCommand
	{
		private bool verbose;
		public VerifySFVCommand (bool verbose)
		{
			this.verbose = verbose;
		}
		public override void Execute ()
		{
		
			FileVerificationManager.Instance.FileCheck += 
				new FileCheckEventHandler (this.OnFileCheck);
			foreach (string s in files){
				FileVerificationManager.Instance.Verify(s);
			}
		}
		protected void OnFileCheck (SFVElement e, FileCheckResult res)
		{
			string msg;
			switch (res){
				case FileCheckResult.Ok: 
					msg = I._("Ok");
					break;
				case FileCheckResult.NotFound:
					msg = I._("Not Found");
					break;
				case FileCheckResult.IsDirectory:
					msg = I._("Is Directory");
					break;
				case FileCheckResult.Failed:
					msg = I._("Failed!");
					break;
				default:
					msg = I._("Unknown error!");
					break;
			}
			if ((verbose) || (res != FileCheckResult.Ok) )
				Console.WriteLine ("{0,-40} {1}", e.FileName + "...", msg);
		}		
	}
}
