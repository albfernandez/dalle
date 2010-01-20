/*

	Dalle - A split/join file utility library
	Dalle.FileVerification.Verifiers.MD5BsdVerifier
		Verifies md5 bsd created files
	
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


using System.IO;
using System;
using System.Collections;

using Dalle.FileVerification.FileHashers;


namespace Dalle.FileVerification.Verifiers
{

	public class MD5BsdVerifier : MD5Verifier
	{
	
		public MD5BsdVerifier ():base ("md5bsd", true)
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
						string fname = linea.Substring (linea.IndexOf ("(")+1, linea.LastIndexOf(')')-5).Trim();
						string hash = linea.Substring (linea.LastIndexOf(" ")).Trim();
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
		
		protected override void PutMd5 (SFVElement e, TextWriter writer)
		{
			writer.WriteLine ("MD5 ({0}) = {1}", e.FileName, e.RealHash);
		}
		public override bool IsFormatRecognized (string file)
		{
			if (!file.ToUpper().EndsWith(".MD5"))
				return false;
			try{
				StreamReader reader = File.OpenText (file);
				string st = reader.ReadLine();
				reader.Close();
				return st.ToUpper().StartsWith("MD5 ");
			}
			catch (Exception){
				
			}
			return false;
		}		
	}
}
