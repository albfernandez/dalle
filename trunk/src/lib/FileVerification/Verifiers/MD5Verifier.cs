/*

	Dalle - A split/join file utility library
	Dalle.FileVerification.Verifiers.MD5Verifier - 	
		Verification of md5 files
	
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


using System.Collections;
using System.IO;
using System;

using Dalle.FileVerification;
using Dalle.FileVerification.FileHashers;


namespace Dalle.FileVerification.Verifiers
{


	public class MD5Verifier : FVerification
	{
		public MD5Verifier (): base ("md5", true)
		{
		}
		protected MD5Verifier (string name, bool canCreate): base (name, canCreate)
		{
		}
		protected override ArrayList GenerateSFVFileList (string file)
		{
			ArrayList ret = new ArrayList ();
			TextReader reader = File.OpenText (file);
			string linea;
			linea = reader.ReadLine ();
			while (linea != null){	
				int idx = linea.IndexOf (';');
				if (idx >=0)
					linea = linea.Substring (0, linea.IndexOf(';'));
				linea = linea.Trim();
				if (linea != string.Empty){
					try{				
					string fname = linea.Substring (linea.IndexOf (" ")).Trim();
					string hash = linea.Substring (0, linea.IndexOf(" ")).Trim();
					fname = fname.Replace ('/', Path.DirectorySeparatorChar);
					SFVElement el = new SFVElement (fname, hash, new FileHasherMd5()); 
					ret.Add (el);
					}
					catch (System.Exception){
					}
				}					
				linea = reader.ReadLine ();
			}
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
					SFVElement e = new SFVElement (f, "", new FileHasherMd5());
					e.GenerateHash();
					PutMd5 (e, writer);										
				}
			}
		}
		protected virtual void PutMd5 (SFVElement e, TextWriter writer)
		{
			writer.WriteLine (e.RealHash + "  " + e.FileName);		
		}
		public override bool IsFormatRecognized (string file)
		{
			if (!file.ToUpper().EndsWith(".MD5"))
				return false;
			try{
				StreamReader reader = File.OpenText (file);
				string st = reader.ReadLine();
				reader.Close();
				return (st.IndexOf(" ") == 32);
			}
			catch (Exception){
				
			}
			return false;
		}
	}
}
