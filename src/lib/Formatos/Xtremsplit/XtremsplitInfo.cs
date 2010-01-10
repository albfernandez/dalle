/*

	Dalle - A split/join file utility library
	Dalle.Formatos.HJSplit.HJSplit - 
		Split and Join files in HJSplit format.
	
    Copyright (C) 2003-2010  Alberto Fernández <infjaf00@yahoo.es>

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
using System.IO;

using Dalle.Utilidades;

namespace Dalle.Formatos.Xstremsplit
{


	public class XtremsplitInfo
	{

		public static readonly int EXE_TAIL_SIZE = 24;
		public static readonly int HEADER_SIZE = 104;
		
		private DirectoryInfo baseDirectory;
		private string baseFilename = "";
		private string programName = "Xtremsplit";
		private string programVersion = "";
		private DateTime creationDate ;
		private string originalFileName = "";
		private bool isMd5 = false;
		private int fragments = 0;
		private long originalSize = 0;
		private bool isExe = false;
		private string md5 = "";
		private int headerOffset = 0;
		private long dataSizeInExe = 0;
		
		public long DataSizeInExe {
			get { return dataSizeInExe;}
		}
		
		public int HeaderOffset {
			get { return headerOffset;}
		}
		
		public DirectoryInfo BaseDirectory {
			get { return baseDirectory;}
		}
		
		public string BaseFilename {
			get { return baseFilename;}
		}
		public string ProgramName {
			get { return programName; }
		}
		public string ProgramVersion {
			get { return programVersion; }
		}
		public DateTime CreationDate {
			get { return creationDate; }
		}
		public string OriginalFileName {
			get { return originalFileName; }
		}
		public bool IsMd5 {
			get { return isMd5; }
		}
		public int Fragments {
			get { return fragments; }
		}
		public long Length {
			get { return originalSize; }
		}
		public bool IsExe {
			get { return isExe; }
		}
		public string MD5 {
			get { return md5; }
			set { md5 = value; }
		}
		
		public XtremsplitInfo ()
		{
		}
		
		public static XtremsplitInfo GetFromFile (string fichero)
		{
			if (!File.Exists (fichero)) 
			{
				return null;
			}
			bool exe = fichero.ToLower ().EndsWith (".001.exe");
			bool xtm = fichero.ToLower ().EndsWith (".001.xtm");
			FileInfo fi = new FileInfo (fichero);
			if (exe || xtm)
			{
				
				XtremsplitInfo info = new XtremsplitInfo ();
				info.baseDirectory = fi.Directory;
				info.headerOffset = 0;
				info.baseFilename = fi.Name.Substring (0, fi.Name.LastIndexOf ('.'));
				info.baseFilename = info.baseFilename.Substring (0, info.baseFilename.LastIndexOf ('.'));
				if (exe) 
				{
					// Leer la cola
					info.isExe = true;
					byte[] tail = UtilidadesFicheros.LeerSeek (fichero, fi.Length - EXE_TAIL_SIZE, EXE_TAIL_SIZE);
					if (tail[0] != 6)
					{
						return null;
					}
					string p = UtArrays.LeerTexto (tail, 1, 6);
					if (!p.Equals ("XTMSFX")) 
					{
						return null;
					}
					if (tail[7] != 3)
					{
						return null;
					}
					p = UtArrays.LeerTexto (tail, 8, 3);
					if (!p.Equals ("1.2"))
					{
						return null;
					}
					info.dataSizeInExe = UtArrays.LeerInt64 (tail, 16);
					info.headerOffset = (int)(fi.Length - EXE_TAIL_SIZE - info.dataSizeInExe - HEADER_SIZE);
					if (info.headerOffset < 100)
					{
						return null;
					}
					
				}
				// Leer la cabecera
				byte[] header = UtilidadesFicheros.LeerSeek (fichero, info.headerOffset, HEADER_SIZE);
				int lonc = header[0];
				info.programName = UtArrays.LeerTexto (header, 1, Math.Min (lonc, 20));
				
				lonc = header[21];
				info.programVersion = UtArrays.LeerTexto (header, 22, Math.Min (lonc, 4));
				
				//36
				info.creationDate = DateTime.Now;
				
				lonc = header[40];
				info.originalFileName = UtArrays.LeerTexto (header, 41, Math.Min (lonc, 50));
				
				info.isMd5 = (header[91] == 1);
				info.fragments = UtArrays.LeerInt32 (header, 92);
				info.originalSize = UtArrays.LeerInt64 (header, 96);
				return info;
			}
			
			
			
			
			return null;
		}
		public string GetFragmentName (int i)
		{
			string extension = "xtm";
			if (i == 1 && this.isExe)
			{
				extension = "exe";
			}
			return 
				baseDirectory.FullName + Path.DirectorySeparatorChar + 
				baseFilename + "." + UtilidadesCadenas.Format (i, 3) + "." + extension;	

		}
		public int GetOffset (int i)
		{
			if (i == 1) 
			{
				return this.headerOffset + HEADER_SIZE;
			}
			return 0;
		}
	}
}
