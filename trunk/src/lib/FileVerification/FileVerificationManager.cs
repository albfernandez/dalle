/*

	Dalle - A split/join file utility library
	Dalle.FileVerification.FileVerificationManager
		Provides a single access point for all supported formats.
	
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

using Dalle.Utilidades;
using Dalle.FileVerification.Verifiers;

namespace Dalle.FileVerification
{

	public class FileVerificationManager 
	{		
		private static FileVerificationManager instance;
	
		private Hashtable supported;
		
		private FileVerificationManager ()
		{
			supported = new Hashtable();
			
			this.Add (new SHA1Verifier());
			this.Add (new MD5Verifier());
			this.Add (new SFVVerifier());
			this.Add (new MD5BsdVerifier());			
			this.Add (new HachaVerifier());
		}
		public void Add (FVerification f)
		{
			supported.Add (f.Name, f);
			f.FileCheck += new FileCheckEventHandler (OnFileCheck);
			f.InitialList += new InitialListEventHandler (OnInitialList);
		}
		
		public static FileVerificationManager Instance {
			get{
				if (instance == null)
					instance = new FileVerificationManager ();
				return instance;
			}
		}
		public void GenerateSFV (string algorithm, ArrayList l, bool recursive, TextWriter writer)
		{
			string[] files = new string[l.Count];
			int i=0;
			foreach (string s in l)
				files[i++]=s;
				
			FVerification verifier = supported[algorithm] as FVerification;
			if (verifier == null){
				throw new Exception ("Format not supported");
			}
			
			verifier.CreateSFV(files, recursive, writer);			
		}
		
		protected void OnFileCheck (SFVElement e, FileCheckResult res)
		{
			if (this.FileCheck != null){
				this.FileCheck (e, res);
			}
		}
		
		protected void OnInitialList (ArrayList l)
		{
			if (this.InitialList != null){
				this.InitialList (l);
			}
		}
		public void Verify (string file)
		{
			IFVerification type = null;
			
			foreach (IFVerification f in supported.Values){
				if (f.IsFormatRecognized (file) ){
					type = f;
					break;
				}
			}
			
			if (type == null){
				// TODO: Custom Exception
				throw new Exception("");
			}
			// TODO: I18N
			Console.WriteLine ("Format = " + type.Name);
			type.VerifySFV (file);
		}	
		public ArrayList GetSupportedFormats ()
		{
			ArrayList l = new ArrayList();
			foreach (IFVerification f in supported.Values){
				if (f.CanCreate){
					l.Add (f);
				}
			}
			return l;
		}
		
		public event InitialListEventHandler InitialList;
		public event FileCheckEventHandler FileCheck;
	}
}
