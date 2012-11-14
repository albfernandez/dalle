/*

    Dalle - A split/join file utility library
	
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

namespace Dalle.Formatos.SplitterMerger
{

	public enum SplitterMergerVersion 
	{
		UNKNOWN,
		VERSION_1_02,
		VERSION_1_1,
		VERSION_2_0,
		VERSION_3_0,
		VERSION_4_0,
		VERSION_4_0_LITE,
		VERSION_5_02,
		VERSION_5_5
	};
	public enum SplitterMergerCompression 
	{
		NO_COMPRESSION,
		V3_COMPRESSION,
		GZIP_COMPRESSION		
	};

	public class SplitterMergerInfo
	{
		private SplitterMergerVersion version;
		private SplitterMergerCompression compression;
		private string originalFilename;
		private string baseFilename;
		private string baseDirectory;
		private long length;
		private DateTime timestamp;
		private int crc;
		private int exeLength;
		private int fragments;
		private bool exe;
		private int splDataSize;
		
		public static readonly int SPL_LENGTH_V_2 = 264;
		public static readonly int SPL_LENGTH_V_3 = 280;
		public static readonly int SPL_LENGTH_V_4 = 288;
		public static readonly int SPL_LENGTH_V_5 = 544;
		public static readonly int OFFSET_V_2 = 0x1600;
		public static readonly int OFFSET_V_3 = 0x1C00;
		public static readonly int OFFSET_V_4 = 0x7200;
		public static readonly int OFFSET_V_4_LITE = 0x3400;
		public static readonly int OFFSET_V_5_02 = 0x7A00;
		public static readonly int OFFSET_V_5_5 = 0x8600;
		
		public SplitterMergerVersion Version {
			get { return version; }
		}
		public SplitterMergerCompression Compression {
			get { return compression; }
		}
		public string OriginalFileName {
			get { return originalFilename; }
		}
		public string BaseFilename {
			get { return baseFilename; }
		}
		public string BaseDirectory {
			get { return baseDirectory;}
		}
		public long Length {
			get { return length; }
		}
		public DateTime Timestamp {
			get { return timestamp; }
		}
		public int Crc {
			get { return crc; }
		}
		public int Fragments {
			get { return fragments; }
		}
		public bool IsExe {
			get { return exe; }
		}
		public int SPLDataSize {
			get { return splDataSize;}
		}
		public bool ValidateCrc {
			get {
				return (crc != 0);
			}
		}

		public SplitterMergerInfo ()
		{
			this.compression = SplitterMergerCompression.NO_COMPRESSION;
		}
		
		public string GetFramentFilename (int fragmentNo)
		{
			if (fragmentNo == 1 && exe) 
			{
				return baseDirectory + Path.DirectorySeparatorChar + baseFilename + ".exe";
			}
			switch (Version) {
			case SplitterMergerVersion.VERSION_1_02:
			case SplitterMergerVersion.VERSION_1_1:
			case SplitterMergerVersion.VERSION_2_0:
				return baseDirectory + Path.DirectorySeparatorChar + baseFilename + "." + fragmentNo;
			case SplitterMergerVersion.VERSION_3_0:
				return baseDirectory + Path.DirectorySeparatorChar + baseFilename + "." + UtilidadesCadenas.Format (fragmentNo, 4);
			case SplitterMergerVersion.VERSION_4_0:
			case SplitterMergerVersion.VERSION_4_0_LITE:
			case SplitterMergerVersion.VERSION_5_02:
			case SplitterMergerVersion.VERSION_5_5:
				return baseDirectory + Path.DirectorySeparatorChar + baseFilename + "." + UtilidadesCadenas.Format (fragmentNo, 3);
			default:
				return baseDirectory + Path.DirectorySeparatorChar + baseFilename + "." + fragmentNo;
			}
			
		}
		public int GetDataOffset (int fragmentNo)
		{
			if (exe && fragmentNo == 1)
			{
				return exeLength + splDataSize;
			}
			return 0;
		}
		
		public static SplitterMergerInfo GetFromSpl (string fichero)
		{
			if (!fichero.ToLower ().EndsWith ("splitter.dat") && !fichero.ToLower ().EndsWith (".spl"))
			{
				return null;
			}
			FileInfo fi = new FileInfo (fichero);
			long size = fi.Length;
			if (size < 2048) 
			{
				byte[] todo = UtilidadesFicheros.LeerTodo (fichero);
				string texto = UtArrays.LeerTexto (todo, 0);
				
				// Version de texto, version 1 o 2
				if (texto.Length == todo.Length) 
				{
					return GetInfoSpl1 (texto, false, fi);
				}
				// v3
				if (todo.Length == SPL_LENGTH_V_3) 
				{
					return GetInfoSpl3 (todo, false, fi);
				}
				// v4
				else if (todo.Length == SPL_LENGTH_V_4)
				{
					return GetInfoSpl4(todo, false, fi);
				}
				// v5
				else if (todo.Length == SPL_LENGTH_V_5)
				{
					return GetInfoSpl5(todo, false,fi);	
				}
					
			}
			return null;
		
		}

		public static SplitterMergerInfo GetFromExe (string fichero)
		{
			
			if (!fichero.ToLower ().EndsWith (".exe")) 
			{
				return null;
			}
			if (!File.Exists (fichero)) 
			{
				return null;
			}
			FileInfo fi = new FileInfo (fichero);
			SplitterMergerVersion v = DetectExeVersion (fichero);
			switch (v) {
			case SplitterMergerVersion.UNKNOWN:
				return null;
			case SplitterMergerVersion.VERSION_2_0:
				byte[] buffer = UtilidadesFicheros.LeerSeek (fichero, OFFSET_V_2, SPL_LENGTH_V_2);
				
				string t = UtArrays.LeerTexto (buffer, 0, SPL_LENGTH_V_2 - 4) + "\n" + UtArrays.LeerTexto (buffer, SPL_LENGTH_V_2 - 4);
				SplitterMergerInfo i = GetInfoSpl1 (t, true, fi);
				i.exeLength = OFFSET_V_2;
				i.exe = true;
				i.version = SplitterMergerVersion.VERSION_2_0;
				return i;
			case SplitterMergerVersion.VERSION_3_0:
				SplitterMergerInfo j = GetInfoSpl3 (UtilidadesFicheros.LeerSeek (fichero, OFFSET_V_3, SPL_LENGTH_V_3), true, fi);
				j.exeLength = OFFSET_V_3;
				j.exe = true;
				return j;
			
			case SplitterMergerVersion.VERSION_4_0:
				SplitterMergerInfo k = GetInfoSpl4 (UtilidadesFicheros.LeerSeek (fichero, OFFSET_V_4, SPL_LENGTH_V_4), true, fi);
				k.exe = true;
				k.version = SplitterMergerVersion.VERSION_4_0;
				return k;
			case SplitterMergerVersion.VERSION_4_0_LITE:
				SplitterMergerInfo l = GetInfoSpl4 (UtilidadesFicheros.LeerSeek (fichero, OFFSET_V_4_LITE, SPL_LENGTH_V_4), true, fi);				l.exe = true;
				l.version = SplitterMergerVersion.VERSION_4_0_LITE;
				l.exeLength = OFFSET_V_4_LITE;
				return l;
			case SplitterMergerVersion.VERSION_5_02:
				SplitterMergerInfo m = GetInfoSpl5 (UtilidadesFicheros.LeerSeek (fichero, OFFSET_V_5_02, SPL_LENGTH_V_5), true, fi);
				m.exe = true;
				m.version = SplitterMergerVersion.VERSION_5_02;
				m.exeLength = OFFSET_V_5_02;
				return m;
			case SplitterMergerVersion.VERSION_5_5:
				SplitterMergerInfo n = GetInfoSpl5 (UtilidadesFicheros.LeerSeek (fichero, OFFSET_V_5_5, SPL_LENGTH_V_5), true, fi);
				n.exe = true;
				n.version = SplitterMergerVersion.VERSION_5_5;
				n.exeLength = OFFSET_V_5_5;
				return n;
			default:
				return null;
			}
		
		}
		public static SplitterMergerInfo GetInfoFromFile (string fichero)
		{
			if (fichero.ToLower ().EndsWith (".exe")) 
			{
				return GetFromExe (fichero);
			}
			return GetFromSpl (fichero);
		}
		public static SplitterMergerVersion DetectExeVersion (string fichero)
		{
			byte[] f = UtilidadesFicheros.LeerSeek (fichero, 0, Math.Min (new FileInfo (fichero).Length, OFFSET_V_5_5 + SPL_LENGTH_V_5));
			
			
			// 5.5
			if (f.Length >= 0x8820)
			{
				if (UtArrays.LeerTexto(f, 0x5DF0, 5).Equals("*GSPL") && UtArrays.LeerInt16(f, 0x881C) == 0x32)
				{
					return 	SplitterMergerVersion.VERSION_5_5;
				}
			}			
			// 5.02
			if (f.Length >= 0x7C20)
			{
				if (UtArrays.LeerTexto(f, 0x564F, 14).Equals("archivo .gz+ N") && UtArrays.LeerInt16(f,0x7C1C) == 0x32)
				{
					return 	SplitterMergerVersion.VERSION_5_02;
				}
			}
			// 4.0 Lite
			if (f.Length >= 0x3520)
			{
				if (UtArrays.LeerTexto(f, 0x19C2,16).Equals("de Splitter & Mg"))
				{
					return SplitterMergerVersion.VERSION_4_0_LITE;
				}
			}
			
			// 4.0
			if (f.Length >= 0x7320)
			{
				if (UtArrays.LeerTexto(f, 0x51A8, 3).Equals("Spl"))
				{
					return SplitterMergerVersion.VERSION_4_0;
				}
			}
			
			// 3.0
			if (f.Length >= 0x1D18)
			{
				if (UtArrays.LeerTexto(f, 0xDF1, 7).Equals("se pued"))
				{
					return SplitterMergerVersion.VERSION_3_0;
				}
			}
			// 2.0
			if (f.Length >= 0x1708)
			{
				if (UtArrays.LeerTexto(f, 0x960, 3).Equals("to!"))
				{
					return SplitterMergerVersion.VERSION_2_0;
				}
			}
			
			return SplitterMergerVersion.UNKNOWN;
		}
		private static SplitterMergerInfo GetInfoSpl1 (string texto, bool esExe, FileInfo fi)
		{
			texto = texto.Replace ("\r", "");
			string[] partes = texto.Split ('\n');
			if (partes.Length != 2 && partes.Length != 3)
			{
				return null;
			}
			string original = partes[0];
			int fragmentos = int.Parse (partes[1]);
			SplitterMergerInfo ret = new SplitterMergerInfo ();
			ret.version = partes.Length == 2 ? SplitterMergerVersion.VERSION_1_02 : SplitterMergerVersion.VERSION_1_1;
			
			ret.originalFilename = original;
			

			ret.baseFilename = fi.Name.Substring (0, fi.Name.LastIndexOf ('.'));
			ret.baseDirectory = fi.Directory.FullName;
			ret.fragments = fragmentos;
			ret.exe = esExe;
			if (esExe)
			{
				ret.version = SplitterMergerVersion.VERSION_2_0;
				ret.exeLength = OFFSET_V_2;
				ret.splDataSize = SPL_LENGTH_V_2;
			}


			return ret;
		}
		private static SplitterMergerInfo GetInfoSpl3 (byte[] buffer, bool esExe, FileInfo fi)
		{
			SplitterMergerInfo ret = new SplitterMergerInfo ();
			if (esExe) 
			{
				ret.exe = esExe;
				ret.exeLength = OFFSET_V_3;
			
			}
			ret.splDataSize = SPL_LENGTH_V_3;
			ret.version = SplitterMergerVersion.VERSION_3_0;
			ret.originalFilename = UtArrays.LeerTexto (buffer, 0, 256);
			ret.baseFilename = ret.originalFilename;
			ret.baseDirectory = fi.Directory.FullName;
			ret.length = UtArrays.LeerInt64(buffer, 256);
			ret.timestamp = UtArrays.LeerDateTime(buffer, 264);
			ret.fragments = UtArrays.LeerInt32(buffer, 272);
			bool comp = UtArrays.LeerInt32(buffer, 276) == 1;
			if (comp) 
			{
				ret.compression = SplitterMergerCompression.V3_COMPRESSION;
				if (ret.originalFilename.ToLower().EndsWith(".spc"))
				{
					ret.originalFilename = ret.originalFilename.Substring(0, ret.originalFilename.LastIndexOf('.'));
				}
			}
			return ret;
		}
		private static SplitterMergerInfo GetInfoSpl4 (byte[] buffer, bool esExe, FileInfo fi)
		{
			SplitterMergerInfo ret = new SplitterMergerInfo ();
			if (esExe) 
			{
				ret.exe = esExe;
				ret.exeLength = OFFSET_V_4;
			
			}
			ret.splDataSize = SPL_LENGTH_V_4;
			ret.version = SplitterMergerVersion.VERSION_4_0;
			ret.originalFilename = UtArrays.LeerTexto (buffer, 0, 256);
			ret.baseFilename = ret.originalFilename;
			ret.baseDirectory = fi.Directory.FullName;
			ret.length = UtArrays.LeerInt64 (buffer, 264);
			ret.timestamp = UtArrays.LeerDateTime (buffer, 272);
			ret.fragments = UtArrays.LeerInt32 (buffer, 280);
			bool comp = UtArrays.LeerInt32 (buffer, 284) == 1;
			if (comp) 
			{
				ret.compression = SplitterMergerCompression.GZIP_COMPRESSION;
				if (ret.originalFilename.ToLower ().EndsWith (".gz"))
				{
					ret.originalFilename = ret.originalFilename.Substring (0, ret.originalFilename.LastIndexOf ('.'));
				}
			}
			ret.fragments = CalculateFragmentsNumber (ret);


			return ret;
		}
		private static SplitterMergerInfo GetInfoSpl5 (byte[] buffer, bool esExe, FileInfo fi)
		{
			SplitterMergerInfo ret = new SplitterMergerInfo ();
			if (esExe) 
			{
				ret.exe = esExe;
				ret.exeLength = OFFSET_V_5_02;
			
			}
			ret.splDataSize = SPL_LENGTH_V_5;
			ret.version = SplitterMergerVersion.VERSION_5_02;
			ret.originalFilename = UtArrays.LeerTexto (buffer, 0, 260);
			ret.baseFilename = UtArrays.LeerTexto (buffer, 260, 260);
			ret.baseDirectory = fi.Directory.FullName;
			ret.length = UtArrays.LeerInt64 (buffer, 520);
			ret.timestamp = UtArrays.LeerDateTime (buffer, 528);
			ret.crc = UtArrays.LeerInt32 (buffer, 536);
			//short _vers = UtArrays.LeerInt16 (buffer, 540);
			bool comp = (UtArrays.LeerInt16 (buffer, 542) == 1);
			if (comp) 
			{
				ret.compression = SplitterMergerCompression.GZIP_COMPRESSION;
				if (ret.originalFilename.ToLower ().EndsWith (".gz"))
				{
					ret.originalFilename = ret.originalFilename.Substring (0, ret.originalFilename.LastIndexOf ('.'));
				}
			}
			
			ret.fragments = CalculateFragmentsNumber (ret);

			return ret;
		}
		private static int CalculateFragmentsNumber (SplitterMergerInfo m)
		{
			int i = 1;
			while (File.Exists (m.GetFramentFilename (i)))
			{
				i++;
			}
			return i - 1;
		}
	}
}
