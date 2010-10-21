/*

	Dalle - A split/join file utility library
	Dalle.FileVerification.Verifiers.SFVVerifier
		Verifies files created with WinSFV or compatible.
	
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
using System.Reflection;

using Dalle.FileVerification;
using Dalle.FileVerification.FileHashers;


namespace Dalle.FileVerification.Verifiers
{
	public class SFVVerifier : FVerification
	{
	
		public SFVVerifier (): base ("sfv", true)
		{
		}
		
		public override bool IsFormatRecognized (string file)
		{
			// TODO - Detect by content instead by extension
			return file.ToUpper().EndsWith (".SFV") || file.ToUpper().EndsWith (".SFV2");
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
				if (linea != string.Empty) {
					try{
						string fname = linea.Substring (0, linea.LastIndexOf (" ")).Trim();
						string hash = linea.Substring (linea.LastIndexOf(" ")).Trim();
						fname = fname.Replace ('\\', Path.DirectorySeparatorChar);
						SFVElement el = new SFVElement (fname, hash, new FileHasherCrc32()); 
						ret.Add (el);
					}
					catch (System.Exception) {
					}
				}				
				linea = reader.ReadLine ();
			}
			return ret;
		}
		protected void CreateRecursive (string[] files, bool recursive, TextWriter writer)
		{
			foreach (string f in files){
				if (Directory.Exists (f)){
					if (recursive){
						CreateRecursive (Directory.GetFileSystemEntries (f), recursive, writer);					
					}
				}
				else {
					SFVElement e = new SFVElement (f, "", new FileHasherCrc32());
					e.GenerateHash();
					writer.WriteLine (f.Replace (Path.DirectorySeparatorChar, '\\') + " " + e.RealHash);
				}
			}
		}
		
		public override void CreateSFV (string[] files, bool recursive, TextWriter writer)
		{
			PutHeader(writer);
			CreateRecursive (files, recursive, writer);
			
		}
		private void PutHeader (TextWriter writer)
		{
			string currentDate = FormatDate();
			writer.WriteLine ("; Generated by WIN-SFV32 v1.1a on " + currentDate);
			writer.WriteLine ("; Bogus line to fool Win-SFV and its lame compatiblity.");
			string m;
			m = "; Generated by " + Assembly.GetEntryAssembly().GetName().Name + " v." +
				Assembly.GetEntryAssembly().GetName().Version + " on " + currentDate;
			writer.WriteLine (m);
			m = "; libDalle v." + Dalle.Formatos.Manager.Version + " - http://dalle.sf.net";
			writer.WriteLine (m);
			
		}
		private string FormatDate ()
		{
			return System.DateTime.Now.ToString();
		}
	
	}
}
