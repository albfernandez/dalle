/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Cutter.CutterTail - Basic support for cutter files.
	
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
/*
struct cutterautoexe	{
						char version[10];
						char crc[50];//del fragmento
						char originalfile[4097];
						long long numberofparts;
						long long exesize; // size of the exe file,in bytes // // Siempre 0
						long long filesize; // size of the packed file,in bytes //
						char ostype[100]; // Operating system typ ; for auto-exe files//
						char cputype[50]; // processor ; for auto-exe files//
						char what[23]; // used to control if it's a valid cutter stuff:-) //
						//record the char* "ohmygodiamacutterstuff" in stuffdata.what , else it will be invalid! //
						};
			*/
using Dalle.Utilidades;
using System.IO;
using System;

namespace Dalle.Formatos.Cutter {


	public class CutterTail {

		public const int TAIL_SIZE = 4360;
		public const string WHAT = "ohmygodiamacutterstuff";
		private long version = 0;
		private long crc = 0;
		private string original;
		private long numberOfParts;
		private long exeSize;
		private long fileSize;
		private string osType;
		private string cpuType;

		
		public long Version {
			get { return version; }
			set { version = value; }
		}
		public long Crc {
			get { return crc; }
			set { crc = value; }
		}
		public string Original {
			get { return original; }
			set { original = value; }
		}
		public long NumberOfParts {
			get { return numberOfParts; }
			set { numberOfParts = value; }
		}
		public long ExeSize {
			get { return exeSize; }
			set { exeSize = value; }
		}
		public long FileSize {
			get { return fileSize; }
			set { fileSize = value; }
		}
		public string OsType {
			get { return osType; }
			set { osType = value; }
		}
		public string CpuType {
			get { return cpuType; }
			set { cpuType = value; }
		}
		public string What {
			get { return WHAT; }
		}
		
		public CutterTail (){
		}
		public static CutterTail LoadFromFile (string fichero)
		{
			try 
			{
				if (!File.Exists (fichero))
				{
					return null;
				}
				FileInfo fi = new FileInfo (fichero);
				if (fi.Length <= TAIL_SIZE) 
				{
					return null;
				}
				FileStream reader = new FileStream (fichero, FileMode.Open);
				reader.Seek (fi.Length - TAIL_SIZE, SeekOrigin.Begin);
				
				byte[] buffer = new byte[TAIL_SIZE];
				reader.Read (buffer, 0, buffer.Length);
				reader.Close ();
				
				CutterTail ret = new CutterTail ();

				ret.version = Convert.ToInt64 (UtArrays.LeerTexto (buffer, 0, 10));
				ret.crc = Convert.ToInt64 (UtArrays.LeerTexto (buffer, 10, 50));
				ret.original = UtArrays.LeerTexto (buffer, 60, 4097);
				ret.numberOfParts = UtArrays.LeerInt64 (buffer, 4160);
				ret.exeSize = UtArrays.LeerInt64 (buffer, 4168);
				ret.fileSize = UtArrays.LeerInt64 (buffer, 4176);
				ret.osType = UtArrays.LeerTexto (buffer, 4184, 100);
				ret.cpuType = UtArrays.LeerTexto (buffer, 4284, 50);
				string what = UtArrays.LeerTexto (buffer, 4334, 23);
				if (what != WHAT)
					return null;
				return ret;
			}
			catch (Exception e) 
			{
				return null;
			}

		}
		public byte[] ToByteArray () {
			
			byte[] ret = new byte [TAIL_SIZE];
			
			UtArrays.EscribirTexto (ret, "" + version, 0);
			UtArrays.EscribirTexto (ret, "" + crc, 10);
			UtArrays.EscribirTexto (ret, original, 60);
			UtArrays.EscribirInt (ret, numberOfParts , 4157);
			UtArrays.EscribirInt (ret, exeSize, 4165);
			UtArrays.EscribirInt (ret, fileSize, 4173);
			UtArrays.EscribirTexto (ret, osType , 4181);
			UtArrays.EscribirTexto (ret, cpuType, 4281);
			UtArrays.EscribirTexto (ret, What, 4331);
			return ret;		
		}	
	}
}
