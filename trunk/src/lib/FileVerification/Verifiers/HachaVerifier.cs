/*

	Dalle - A split/join file utility library
	Dalle.FileVerification.Verifiers.HachaVerifier
		Verifies HachaPro crc files.
	
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



using System.Collections;
using System.IO;
using System;

using Dalle.FileVerification;
using Dalle.FileVerification.FileHashers;

namespace Dalle.FileVerification.Verifiers
{
	public class HachaVerifier : FVerification
	{
		public HachaVerifier (): base ("hacha", true)
		{
		}
		protected override ArrayList GenerateSFVFileList (string file)
		{
			ArrayList ret = new ArrayList ();
			
			return ret;
		}
		public override void CreateSFV (string[] files, bool recursive, TextWriter writer)
		{
			CreateRecursive (files, recursive, writer);			
		}
		protected void CreateRecursive (string[] files, bool recursive, TextWriter writer)
		{
			foreach (string f in files){
				if (Directory.Exists (f)){
					if (recursive)
						CreateRecursive (Directory.GetFileSystemEntries (f), recursive, writer);					
				}
				else {
					SFVElement e = new SFVElement (f, "", new FileHasherHacha());
					e.GenerateHash();
					PutHash (e, writer);										
				}
			}
		}
		protected void PutHash (SFVElement e, TextWriter writer)
		{
			writer.WriteLine (e.RealHash + "  " + e.FileName);		
		}		
	}
}
