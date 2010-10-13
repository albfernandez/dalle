/*

	Dalle - A split/join file utility library
	Dalle.Formatos.HJSplit.HJSplit - 
		Split and Join files in HJSplit format.
	
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



using System;
using System.IO;

using Dalle.Utilidades;

namespace Dalle.Formatos.Yoyocut
{
	public class YoyocutInfo
	{
		private int numeroPartes = 0;
		public int Fragments {
			get { return numeroPartes; }
		}
		
		private String extension = "";
		public String Extension {
			get { return extension; }
			set { extension = value; }
		}
		private String md5 = null;
		public String Md5 {
			get { return md5; }
			set { md5 = value; }
		}
		public bool IsMd5 
		{
			get { return md5 != null;}
		}
		public YoyocutInfo ()
		{
		}
		private String baseFilename;
		private DirectoryInfo baseDirectory;
		private const int EXE_TAIL_SIZE = 2;
		private const int EXE_SIZE = 0x2E00;
		
		private int exeSize = 0;
		private bool isExe = false;
		public bool IsExe {
			get { return isExe;}
		}
		private String originalExtension;
		public String OriginalFileName {
			get { return baseFilename + "." + originalExtension; }
		}
		private long length;
		public long Length {
			get { return length; }
		}
		private long dataSizeInExe = 0;
		public long DataSizeInExe {
			get { return dataSizeInExe; }
		}
		public long HeaderSize 
		{
			get {
				long tmp = 0;
				tmp = originalExtension.Length + 1 + 3;
				if (this.IsMd5) 
				{
					tmp += 4 + 32;
				}
				return tmp;
			}
		}
		
		public static YoyocutInfo GetFromFile (string fichero)
		{
			if (!File.Exists (fichero)) {
				return null;
			}
			bool exe = fichero.ToLower ().EndsWith (".001.exe");
			bool yct = fichero.ToLower ().EndsWith (".001.yct");
			FileInfo fi = new FileInfo (fichero);
			if (exe || yct) {
				YoyocutInfo info = new YoyocutInfo ();
				info.baseDirectory = fi.Directory;

				info.baseFilename = fi.Name.Substring (0, fi.Name.LastIndexOf ('.'));
				info.baseFilename = info.baseFilename.Substring (0, info.baseFilename.LastIndexOf ('.'));
				if (exe) {
					
					info.isExe = true;
					
					
										
					// Exe -> acaba en 00 2E
					
					byte[] tail = UtilidadesFicheros.LeerSeek (fichero, fi.Length - EXE_TAIL_SIZE, EXE_TAIL_SIZE);
					if (tail[0] != 0 || tail[1] != 0x2E) {
						return null;
					}
					info.exeSize = EXE_SIZE;
					byte[] exeContent = UtilidadesFicheros.LeerSeek (fichero, 0, info.exeSize);
					String texto = UtArrays.LeerTexto (exeContent, 0x20DF, 6);
					if (!"oyoCut".Equals (texto))
					{
						return null;
					}

					//Magic - 0x20DF -> oyoCut

					
				}
				
				byte[] header = UtilidadesFicheros.LeerSeek (fichero, info.exeSize, 100 + 3 + 32 + 4);
				
				//UtArrays.LeerTexto (header, 0);
				int posEspacio = 0;
				while (posEspacio < 100 && header[posEspacio] != 32)
				{
					posEspacio++;
				}
				if (posEspacio >= 100)
				{
					return null;
				}
				info.originalExtension = UtArrays.LeerTexto (header, 0, posEspacio);
				String num = UtArrays.LeerTexto (header, posEspacio + 1, 3);
				try 
				{
					info.numeroPartes = Int32.Parse (num);
				}
				catch (Exception) {
					return null;
				}
				
				// Ahora el md5
				String _md5 = UtArrays.LeerTexto (header, posEspacio + 1 + 3, 4);
				if ("MD5:".Equals (_md5))
				{
					info.md5 = UtArrays.LeerTexto (header, posEspacio + 1 + 3 + 4, 32);
				}
				info.length = fi.Length - info.HeaderSize;
				if (info.isExe) 
				{
					info.dataSizeInExe = fi.Length - info.HeaderSize - 2 - info.exeSize;
					info.length = info.dataSizeInExe;
				}
				for (int i = 2; i <= info.Fragments; i++) 
				{
					fi = new FileInfo (info.GetFragmentName (i));
					info.length += fi.Length;
				}
				
				return info;
			}
			
			return null;
		}
		public string GetFragmentName (int i)
		{
			string extension = "yct";
			if (i == 1 && this.isExe) {
				extension = "exe";
			}
			return baseDirectory.FullName + Path.DirectorySeparatorChar + baseFilename + "." + UtilidadesCadenas.Format (i, 3) + "." + extension;
			
		}
		public long GetOffset (int i)
		{
			if (i == 1) 
			{
				return this.HeaderSize + this.exeSize;
			}

			return 0;
		}
		public int GetInfoLastBytes (int i)
		{
			if (i == 1 && this.isExe)
			{
				return 2;
			}
			return 0;
		}
	}
}

