/*

	Dalle - A split/join file utility library
	Dalle.FileVerification.Verifiers.MD5BsdVerifier
		Verifies md5 bsd created files
	
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
				string fname = linea.Substring (linea.IndexOf ("(")+1, linea.LastIndexOf(')')-5).Trim();
				string hash = linea.Substring (linea.LastIndexOf(" ")).Trim();
				fname = fname.Replace ('/', Path.DirectorySeparatorChar);
				SFVElement el = new SFVElement (fname, hash, new FileHasherMd5()); 
				ret.Add (el);
				
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
