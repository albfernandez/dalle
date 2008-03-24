/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Cutter.CutterTail - Basic support for cutter files.
	
    Copyright (C) 2004  Alberto Fernández <infjaf00@yahoo.es>

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
		public static CutterTail LoadFromFile (string fichero) {
			FileStream reader = new FileStream (fichero, FileMode.Open);
			if (reader.Length <= TAIL_SIZE){
				reader.Close();
				return null;
			}
			reader.Seek (reader.Length - TAIL_SIZE, SeekOrigin.Begin);
			
			byte[] buffer = new byte[TAIL_SIZE];
			reader.Read (buffer, 0, TAIL_SIZE);
			reader.Close();		
			
			CutterTail ret = new CutterTail();
			
			ret.version = Convert.ToInt64(UtArrays.LeerTexto (buffer, 0));
			ret.crc = Convert.ToInt64(UtArrays.LeerTexto (buffer, 10));
			ret.original = UtArrays.LeerTexto(buffer, 60);
			ret.numberOfParts = UtArrays.LeerInt64 (buffer, 4157);
			ret.exeSize = UtArrays.LeerInt64 (buffer, 4165);
			ret.fileSize = UtArrays.LeerInt64(buffer, 4173);
			ret.osType = UtArrays.LeerTexto(buffer, 4181);
			ret.cpuType= UtArrays.LeerTexto(buffer, 4281);
			
			string what = UtArrays.LeerTexto(buffer, 4331);
			if (what != WHAT)
				return null;
			return ret;
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
